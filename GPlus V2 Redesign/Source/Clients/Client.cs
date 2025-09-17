using CoreRCON;
using CoreRCON.Parsers.Standard;
using GPlus.Game.Servers;
using GPlus.Source;
using GPlus.Source.GMOD;
using GPlus.Source.Sandboxing;
using GPlus.Source.Structs;
using System.Diagnostics;
using System.Management;
using System.Net;

namespace GPlus.Game.Clients
{
    internal sealed class Client : IDisposable
    {
        public LoginDetails LoginDetails { get; }
        public Server? ConnectedServer { get; private set; }
        public Sandboxie Environment { get; }
        public ushort RCONPort { get; private set; }
        public RCON? RCON { get; private set; }
        public GMOD GMOD { get; set; } = new();

        public bool IsConnected => ConnectedServer != null;

        public event EventHandler? OnSteamStarted;
        public event EventHandler? OnGMODStarted;

        private Process? _steam;
        private Process? _gmod;

        #region Private Methods

        private void Connect()
        {
            if (RCON?.Connected == true && ConnectedServer != null)
                RCON.SendCommandAsync($"connect {ConnectedServer.IP}");
        }

        private void Disconnect()
        {
            if (RCON?.Connected == true)
                RCON.SendCommandAsync("disconnect");
        }

        private async Task<Status?> GetCurrentStatusAsync()
        {
            return RCON == null ? null : await RCON.SendCommandAsync<Status>("status");
        }

        private static IEnumerable<Process> GetChildProcessesRecursive(Process parent)
        {
            var children = new List<Process>();

            try
            {
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = {parent.Id}"
                );

                foreach (var obj in searcher.Get())
                {
                    if (obj["ProcessId"] is int pid)
                    {
                        try
                        {
                            var child = Process.GetProcessById(pid);
                            children.Add(child);
                            children.AddRange(GetChildProcessesRecursive(child));
                        }
                        catch { /* Ignore missing process */ }
                    }
                }
            }
            catch { /* Ignore WMI failures */ }

            return children;
        }

        private static string GetCommandLine(Process process)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"
                );

                foreach (var obj in searcher.Get())
                    return obj["CommandLine"]?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        private async Task<bool> WaitForSteamLoginAsync(Process steam)
        {
            Debug.WriteLine($"[Client] Waiting for Steam login for {LoginDetails.Username} (PID {steam.Id})...");

            while (!steam.HasExited)
            {
                try
                {
                    foreach (var child in GetChildProcessesRecursive(steam))
                    {
                        if (!child.ProcessName.Equals("steamwebhelper", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var cmdLine = GetCommandLine(child);
                        if (cmdLine.Contains("--steamid=") && !cmdLine.Contains("--steamid=0"))
                        {
                            Debug.WriteLine($"[Client] Steam login confirmed for {LoginDetails.Username}.");
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Client] Error scanning child processes: {ex.Message}");
                }

                await Task.Delay(1000);
            }

            Debug.WriteLine($"[Client] Steam exited before login for {LoginDetails.Username}");
            return false;
        }

        private async Task CreateRCONConnectionAsync()
        {
            RCON = new RCON(
                IPAddress.Loopback,
                RCONPort,
                SettingsManager.CurrentSettings.General.RCONPassword
            );

            await RCON.ConnectAsync();
            Environment.RconConnection = RCON;
        }

        private void InitialiseClientLoop() => Task.Run(ClientLoop);

        private async Task ClientLoop()
        {
            Debug.WriteLine($"[Client] Starting client loop for {LoginDetails.Username}");

            while (_steam != null && !_steam.HasExited && _gmod != null && !_gmod.HasExited)
            {
                if (RCON == null)
                {
                    await CreateRCONConnectionAsync();
                    await Task.Delay(10000);
                    continue;
                }

                if (ConnectedServer != null)
                {
                    var status = await GetCurrentStatusAsync();
                    if (status == null || string.IsNullOrEmpty(status.PublicHost) || !status.PublicHost.Contains(ConnectedServer.IP))
                        Connect();
                }

                await Task.Delay(5000);
            }

            Debug.WriteLine($"[Client] Exiting client loop for {LoginDetails.Username}");
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
                    $"-login {LoginDetails.Username} {LoginDetails.Password}",
                    Environment.SandboxName
                );

                if (processResult.Data == null)
                {
                    Debug.WriteLine($"[Client] Failed to start Steam for {LoginDetails.Username}");
                    return false;
                }

                _steam = processResult.Data;
                Debug.WriteLine($"[Client] Steam launched (PID {_steam.Id}) for {LoginDetails.Username}");

                if (!await WaitForSteamLoginAsync(_steam))
                    return false;

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

        public void StopGMOD()
        {
            if (_gmod == null || _gmod.HasExited || _steam == null || _steam.HasExited) return;

            try
            {
                if (RCON?.Connected == true)
                    RCON.SendCommandAsync("quit");

                RCON?.Dispose();
                _gmod.Close();
            }
            catch { /* Ignore */ }
            finally
            {
                _gmod?.Dispose();
            }
        }

        public void SetGMOD(uint processId) => _gmod = Process.GetProcessById((int)processId);

        public bool IsSteamOpen() => _steam != null && !_steam.HasExited;
        public bool IsGMODOpen() => _gmod != null && !_gmod.HasExited;

        public Client(LoginDetails loginDetails, Sandboxie environment)
        {
            LoginDetails = loginDetails;
            Environment = environment;

            Debug.WriteLine($"[Client] Creating client for {loginDetails.Username}");

            Task.Run(async () =>
            {
                if (!await InitialiseSteamAsync()) return;
            });

            OnGMODStarted += (_, _) => InitialiseClientLoop();
        }

        public void Dispose()
        {
            Disconnect();
            RCON?.Dispose();
            StopGMOD();
            StopSteam();
        }

        #endregion
    }
}
