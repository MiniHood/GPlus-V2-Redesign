using CoreRCON;
using CoreRCON.Parsers.Standard;
using GPlus_V2_Redesign.Game.Servers;
using GPlus_V2_Redesign.GUI.Elements;
using GPlus_V2_Redesign.Source;
using GPlus_V2_Redesign.Source.Sandboxie;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8601 // Possible null reference assignment.


namespace GPlus_V2_Redesign.Game.Clients
{
    internal static class Extensions
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        public static string? GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            var fileNameBuilder = new StringBuilder(buffer);
            uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
            return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
                fileNameBuilder.ToString() :
                null;
        }
    }
    internal class Client
    {
        public LoginDetails LoginDetails;
        public Server? ConnectedServer;
        public Sandboxie Enviroment;
        public ushort RCONPort;
        public RCON? RCON;

        public bool IsConnected => ConnectedServer != null;

        public event EventHandler? OnSteamStarted;
        public event EventHandler? OnGMODStarted;

        private bool IsSteamOpen = false;
        private bool IsGMODOpen = false;
        private Process Steam;
        private Process GMOD;


        private void Connect()
        {
            if (RCON == null || RCON.Connected == false)
                return;

            if (ConnectedServer == null)
                return;

            RCON.SendCommandAsync($"connect {ConnectedServer.IP}");
        }

        private void Disconnect()
        {
            if (RCON == null || RCON.Connected == false)
                return;

            RCON.SendCommandAsync($"disconnect");
        }

        private async Task<Status?> GetCurrentStatus()
        {
            if (RCON == null)
                return null;

            Status status = await RCON.SendCommandAsync<Status>("status");
            return status;
        }

        private static IEnumerable<Process> GetChildProcessesRecursive(Process parent)
        {
            var children = new List<Process>();

            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    $"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = {parent.Id}"))
                {
                    foreach (var @object in searcher.Get())
                    {
                        int pid = Convert.ToInt32(@object["ProcessId"]);
                        try
                        {
                            var child = Process.GetProcessById(pid);
                            children.Add(child);

                            children.AddRange(GetChildProcessesRecursive(child));
                        }
                        catch
                        {
                           
                        }
                    }
                }
            }
            catch
            {
                
            }

            return children;
        }



        private static string GetCommandLine(Process process)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"))
                {
                    foreach (var @object in searcher.Get())
                    {
                        return @object["CommandLine"]?.ToString() ?? string.Empty;
                    }
                }
            }
            catch
            {
                // Fallback if WMI access fails
            }

            return string.Empty;
        }


        private async Task<bool> WaitForSteamLoginAsync(Process steam)
        {
#if DEBUG
            Debug.WriteLine($"[Client] Waiting for Steam login for {LoginDetails.Username} (PID {steam.Id})...");
#endif

            while (!steam.HasExited)
            {
                try
                {
                    foreach (var child in GetChildProcessesRecursive(steam))
                    {
#if DEBUG
                        Debug.WriteLine($"[Client] Checking process: {child.ProcessName} (PID {child.Id})");
#endif

                        if (child.ProcessName.Equals("steamwebhelper", StringComparison.OrdinalIgnoreCase))
                        {
                            string cmdLine = GetCommandLine(child);
#if DEBUG
                            Debug.WriteLine($"[Client] steamwebhelper args: {cmdLine}");
#endif

                            if (cmdLine.Contains("--steamid=") && !cmdLine.Contains("--steamid=0"))
                            {
#if DEBUG
                                Debug.WriteLine($"[Client] Steam login confirmed for {LoginDetails.Username} (SteamID detected).");
#endif
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine($"[Client] Error while scanning child processes: {ex.Message}");
#endif
                }

                await Task.Delay(1000);
            }

#if DEBUG
            Debug.WriteLine($"[Client] Steam exited before login for {LoginDetails.Username}");
#endif
            return false;
        }



        public async Task<bool> InitialiseSteamAsync()
        {
            try
            {
                Debug.WriteLine($"[Client] Launching Steam for {LoginDetails.Username}...");

                var processResult = SandboxieWrapper.RunBoxed(
                    SettingsManager.CurrentSettings.General.SteamPath,
                    $"-login {LoginDetails.Username} {LoginDetails.Password}",
                    Enviroment._sandboxName
                );

                if (processResult.Data == null)
                {
#if DEBUG
                    Debug.WriteLine($"[Client] Failed to start Steam process for {LoginDetails.Username}");
#endif
                    return false;
                }

                Steam = processResult.Data;
#if DEBUG

                Debug.WriteLine($"[Client] Steam launched with PID {Steam.Id} for {LoginDetails.Username} at {Steam.GetMainModuleFileName()}");
#endif

                bool loggedIn = await WaitForSteamLoginAsync(Steam);

                if (!loggedIn)
                {
#if DEBUG
                    Debug.WriteLine($"[Client] Steam login failed or exited for {LoginDetails.Username}");
#endif
                    return false;
                }

                IsSteamOpen = true;
#if DEBUG
                Debug.WriteLine($"[Client] Steam is fully logged in for {LoginDetails.Username}");
#endif

                OnSteamStarted?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Client] Exception while starting Steam for {LoginDetails.Username}: {ex}");
                return false;
            }
        }

        public void StopSteam()
        {
            if (Steam == null || Steam.HasExited)
                return;
            Steam.Close();
            Steam.Dispose();
        }

        public async Task<bool> InitialiseGMODAsync()
        {
            if (!IsSteamOpen)
            {
#if DEBUG
                Debug.WriteLine("[Client] Waiting for Steam to start...");
#endif
                while (!IsSteamOpen)
                    await Task.Delay(500);
            }

            var processResult = SandboxieWrapper.RunBoxed(
                Path.Combine(
                    SettingsManager.CurrentSettings.General.GMODDirectory,
                    SettingsManager.CurrentSettings.General.GMODExecutable
                ),
                SettingsManager.CurrentSettings.General.GMODLaunchArguments,
                Enviroment._sandboxName
            );


#if DEBUG
            Debug.WriteLine(Path.Combine(
                    SettingsManager.CurrentSettings.General.GMODDirectory,
                    SettingsManager.CurrentSettings.General.GMODExecutable
                ).ToString());
            Debug.WriteLine($"Attempting to start garry\'s mod.  {processResult.Data}");
#endif
            if (processResult.Data == null)
                return false;

            GMOD = processResult.Data;
            IsGMODOpen = true;
            OnGMODStarted?.Invoke(this, EventArgs.Empty);
#if DEBUG
            Debug.WriteLine($"[Client] GMOD started for {LoginDetails.Username} with PID {GMOD.Id}");
#endif
            return true;
        }


        public void StopGMOD()
        {
            if (GMOD == null || GMOD.HasExited || Steam == null || Steam.HasExited)
                return;
            if(RCON != null && RCON.Connected)
                RCON.SendCommandAsync("quit");
            if (RCON != null)
                RCON.Dispose();
            try { GMOD.Close(); } catch { }
            GMOD.Dispose();
        }

        private async Task ClientLoop()
        {
            Debug.WriteLine($"[Client] Starting client loop for {LoginDetails.Username}");
            while (true)
            {
                if(Steam == null)
                {
#if DEBUG
                    Debug.WriteLine($"[Client] Steam process is null, exiting client loop for {LoginDetails.Username}");
#endif
                    break;
                }
                
                if(GMOD == null)
                {
#if DEBUG
                    Debug.WriteLine($"[Client] GMOD process is null, exiting client loop for {LoginDetails.Username}");
#endif
                    break;
                }

                while(RCON == null)
                {
                    await CreateRCONConnection();
                    await Task.Delay(10000);
                }

                if(ConnectedServer != null)
                {
                    Status? status = await GetCurrentStatus();
                    if (status != null && !status.PublicHost.Contains(ConnectedServer.IP))
                        Connect();
                    else if(status == null || status.PublicHost == null)
                        Connect();
                }
            }
        }

        private async Task CreateRCONConnection()
        {
            RCON = new RCON(IPAddress.Parse("127.0.0.1"), port: RCONPort, password: SettingsManager.CurrentSettings.General.RCONPassword);
            await RCON.ConnectAsync();
            Enviroment._rconConnection = RCON;
        }

        private async void InitialiseClientLoop()
        {
            await Task.Run(() => ClientLoop());
        }

        public Client(LoginDetails loginDetails, Sandboxie enviroment)
        {
            LoginDetails = loginDetails;
            Enviroment = enviroment;
            ConnectedServer = null;

#if DEBUG
            Debug.WriteLine($"[Client] Creating client for {loginDetails.Username}");
#endif
            Task.Run(async () =>
            {
                bool steamStarted = await InitialiseSteamAsync();
                if (!steamStarted)
                    return;
            });

            OnSteamStarted += async (sender, e) =>
            {
                await InitialiseGMODAsync();
            };

#if DEBUG
            Debug.WriteLine($"[Client] Subscribed too OnSteamStarted");
#endif
            OnGMODStarted += (sender, e) =>
            {
                InitialiseClientLoop();
            };
#if DEBUG
            Debug.WriteLine($"[Client] Subscribed too OnGMODStarted");
#endif
        }


        ~Client()
        {
            Disconnect();
            RCON?.Dispose();
            StopGMOD();
            StopSteam();
        }

    }
}
