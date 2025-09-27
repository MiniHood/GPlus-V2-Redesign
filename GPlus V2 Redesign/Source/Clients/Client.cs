using GPlus.Game.Servers;
using GPlus.Source.General;
using GPlus.Source.GMOD;
using GPlus.Source.Interprocess;
using GPlus.Source.Sandboxing;
using GPlus.Source.Steam;
using GPlus.Source.Structs;
using System.Diagnostics;
using System.Management;
using System.Net;
using static SandboxieWrapper;


namespace GPlus.Game.Clients
{
    internal sealed class Client : IDisposable
    {
        public LoginDetails LoginDetails { get; }
        public Server? ConnectedServer { get; private set; }
        public Sandboxie Environment { get; }
        public ushort RCONPort { get; private set; }
        public GMOD GMOD { get; set; } = new();

        public bool IsConnected => ConnectedServer != null;

        public event EventHandler? OnSteamStarted;
        public event EventHandler? OnGMODStarted;

        private Process? _steam;
        private Process? _gmod;

        #region Private Methods


        private async Task<bool> WaitForSteamLoginAsync(Process steam, int timeoutSeconds = 120)
        {
            Debug.WriteLine($"[Client] Waiting for Steam login for {LoginDetails.Username} (PID {steam.Id})...");

            var sw = Stopwatch.StartNew();

            while (!steam.HasExited)
            {
                try
                {
                    foreach (var child in ProcessHelpers.GetChildProcessesRecursive(steam.Id))
                    {
                        string cmdLine = ProcessHelpers.GetCommandLine(child);
                        Debug.WriteLine(cmdLine);
                        if (string.IsNullOrEmpty(cmdLine))
                            continue;

                        if (cmdLine.Contains("--steamid=") && !cmdLine.Contains("--steamid=0"))
                        {
                            Debug.WriteLine($"[Client] Steam login confirmed for {LoginDetails.Username} (PID {child.Id}).");
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Client] Error scanning child processes: {ex.Message}");
                }

                if (sw.Elapsed.TotalSeconds > timeoutSeconds)
                {
                    Debug.WriteLine($"[Client] Timeout waiting for Steam login for {LoginDetails.Username}.");
                    return false;
                }

                await Task.Delay(1000);
            }

            Debug.WriteLine($"[Client] Steam process exited before login detected for {LoginDetails.Username}.");
            return false;
        }

        #endregion

        #region Public API

        public async Task<bool> InitialiseSteamAsync()
        {
            try
            {
                Debug.WriteLine($"[Client] Launching Steam for {LoginDetails.Username}...");

                var processResult = SandboxieWrapper.RunBoxed(
                    SettingsManager.CurrentSettings.General.SteamPath,
                    new ProcessStartInfo { 
                        Arguments =$"-login {LoginDetails.Username} {LoginDetails.Password} -silent -nobootstrapupdate -nocrashdialog -noverifyfiles"
                    },
                    Environment.SandboxName
                );

                if (processResult.Data == null)
                {
                    Debug.WriteLine($"[Client] Failed to start Steam for {LoginDetails.Username}");
                    return false;
                }

                _steam = processResult.Data;
                Debug.WriteLine($"[Client] Steam launched (PID {_steam.Id}) for {LoginDetails.Username}");

                while (!await WaitForSteamLoginAsync(_steam))
                    await Task.Delay(1000);

                OnSteamStarted?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Client] Error starting Steam for {LoginDetails.Username}: {ex}");
                return false;
            }
        }

        public void StopSteam()
        {
            if (_steam == null || _steam.HasExited) return;
            _steam.Close();
            _steam.Dispose();
        }


        private void SetupGMODCommunication()
        {
            try
            {
                Task.Run(() =>
                {
                    Memory.InjectDll((uint)GMOD.Process.Id, "Communication.dll");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SetupGMODCommunication exception: {ex.Message}");
            }
        }


        private async Task SearchForGMOD()
        {
            while (true)
            {
                var sbResult = SandboxieWrapper.GetBoxedProcesses(Environment.SandboxName);
                var list = sbResult.Data;

                if (list == null)
                    throw new Exception($"Could not retrieve box processes for {Environment.SandboxName}");

                foreach (var parent in list)
                {
                    try
                    {
                        Debug.WriteLine($"[SearchForGMOD] Checking sandboxed parent {parent.ProcessName} ({parent.Id})");

                        foreach (var child in ProcessHelpers.GetChildProcessesRecursive(parent.Id))
                        {
                            try
                            {
                                Debug.WriteLine($"[SearchForGMOD] └─ Child {child.ProcessName} ({child.Id})");

                                if (child.ProcessName.Equals("gmod", StringComparison.OrdinalIgnoreCase) &&
                                    child.MainWindowHandle != IntPtr.Zero)
                                {
                                    GMOD.Process = child;
                                    _gmod = child;
                                    Debug.WriteLine($"[SearchForGMOD] Found GMOD (PID {child.Id})");
                                    SetupGMODCommunication();
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"[SearchForGMOD] Failed to inspect child: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[SearchForGMOD] Failed to inspect parent: {ex.Message}");
                    }
                }

                await Task.Delay(250);
            }
        }




        public async Task StartGMODAsync()
        {
            while(true)
            {
                bool Found = false;
                foreach (var child in ProcessHelpers.GetChildProcessesRecursive(_steam.Id))
                {
                    string cmdline = ProcessHelpers.GetCommandLine(child);
                    if (cmdline.ToLower().Contains("--type=renderer") && cmdline.ToLower().Contains("--user-data-dir"))
                    {
                        Found = true;
                        break;
                    }
                    Debug.WriteLine($"Checking {child.Id} for specific arguments.");
                }

                if (Found)
                    break;

                await Task.Delay(300);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = $"{Application.StartupPath}SteamCMD\\steamcmd.exe",
                Arguments =
                    $"+force_install_dir {Application.StartupPath}SteamCMD\\gmod " +
                    $"+login {LoginDetails.Username} {LoginDetails.Password} " +
                    "+app_run 4000 " +
                    $"{SettingsManager.CurrentSettings.General.GMODLaunchArguments}",
                CreateNoWindow = true,
            };

            var b_RESULT = SandboxieWrapper.RunBoxed($"{Application.StartupPath}SteamCMD\\steamcmd.exe", startInfo, Environment.SandboxName);
            if (b_RESULT.Data == null)
                throw new Exception("Failed to get GMOD process from Sandboxie");

            Process proc = b_RESULT.Data;

            // Wait for GMOD to start inside SteamCMD
            await SearchForGMOD();
        }


        public bool IsSteamOpen() => _steam != null && !_steam.HasExited;
        public bool IsGMODOpen() => _gmod != null && !_gmod.HasExited;

        public Client(LoginDetails loginDetails, Sandboxie environment)
        {
            LoginDetails = loginDetails;
            Environment = environment;
            OnSteamStarted += (_, _) => StartGMODAsync();
            //OnGMODStarted += (_, _) => InitialiseClientLoop();
        }

        public void Dispose()
        {
            StopSteam();
        }

        #endregion
    }
}
