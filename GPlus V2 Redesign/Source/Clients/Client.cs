using CoreRCON;
using CoreRCON.Parsers.Standard;
using GPlus.Game.Servers;
using GPlus.Source.General;
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
