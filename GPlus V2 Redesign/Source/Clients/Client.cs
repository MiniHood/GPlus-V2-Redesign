using CoreRCON;
using CoreRCON.Parsers.Standard;
using GPlus.Game.Servers;
using GPlus.Source;
using GPlus.Source.Sandboxing;
using GPlus.Source.Structs;
using System.Diagnostics;
using System.Management;
using System.Net;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference argument.

namespace GPlus.Game.Clients
{
    internal class Client
    {
        public LoginDetails LoginDetails;
        public Server? ConnectedServer;
        public Sandboxie Enviroment;
        public ushort RCONPort = 0;
        public RCON? RCON;

        public bool IsConnected => ConnectedServer != null;

        public event EventHandler? OnSteamStarted;
        public event EventHandler? OnGMODStarted;

        #region Private

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
                using (
                    var searcher = new ManagementObjectSearcher(
                        $"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = {parent.Id}"
                    )
                )
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
                        catch { }
                    }
                }
            }
            catch { }

            return children;
        }

        private static string GetCommandLine(Process process)
        {
            try
            {
                using (
                    var searcher = new ManagementObjectSearcher(
                        $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"
                    )
                )
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
            Debug.WriteLine(
                $"[Client] Waiting for Steam login for {LoginDetails.Username} (PID {steam.Id})..."
            );

            while (!steam.HasExited)
            {
                try
                {
                    foreach (var child in GetChildProcessesRecursive(steam))
                    {
                        Debug.WriteLine(
                            $"[Client] Checking process: {child.ProcessName} (PID {child.Id})"
                        );

                        if (
                            child.ProcessName.Equals(
                                "steamwebhelper",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            string cmdLine = GetCommandLine(child);

                            Debug.WriteLine($"[Client] steamwebhelper args: {cmdLine}");

                            if (cmdLine.Contains("--steamid=") && !cmdLine.Contains("--steamid=0"))
                            {
                                Debug.WriteLine(
                                    $"[Client] Steam login confirmed for {LoginDetails.Username} (SteamID detected)."
                                );

                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Client] Error while scanning child processes: {ex.Message}");
                }

                await Task.Delay(1000);
            }

            Debug.WriteLine($"[Client] Steam exited before login for {LoginDetails.Username}");

            return false;
        }

        private async Task CreateRCONConnection()
        {
            RCON = new RCON(
                IPAddress.Parse("127.0.0.1"),
                port: RCONPort,
                password: SettingsManager.CurrentSettings.General.RCONPassword
            );
            await RCON.ConnectAsync();
            Enviroment._rconConnection = RCON;
        }

        private async void InitialiseClientLoop()
        {
            await Task.Run(() => ClientLoop());
        }

        private async Task ClientLoop()
        {
            Debug.WriteLine($"[Client] Starting client loop for {LoginDetails.Username}");

            while (true)
            {
                if (Steam == null)
                {
                    Debug.WriteLine(
                        $"[Client] Steam process is null, exiting client loop for {LoginDetails.Username}"
                    );
                    break;
                }

                if (GMOD == null)
                {
                    Debug.WriteLine(
                        $"[Client] GMOD process is null, exiting client loop for {LoginDetails.Username}"
                    );
                    break;
                }

                while (RCON == null)
                {
                    await CreateRCONConnection();
                    await Task.Delay(10000);
                }

                if (ConnectedServer != null)
                {
                    Status? status = await GetCurrentStatus();
                    if (status != null && !status.PublicHost.Contains(ConnectedServer.IP))
                        Connect();
                    else if (status == null || status.PublicHost == null)
                        Connect();
                }
            }
        }

        #endregion

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
                    Debug.WriteLine(
                        $"[Client] Failed to start Steam process for {LoginDetails.Username}"
                    );

                    return false;
                }

                Steam = processResult.Data;

                Debug.WriteLine(
                    $"[Client] Steam launched with PID {Steam.Id} for {LoginDetails.Username}"
                );

                bool loggedIn = await WaitForSteamLoginAsync(Steam);

                if (!loggedIn)
                {
                    Debug.WriteLine(
                        $"[Client] Steam login failed or exited for {LoginDetails.Username}"
                    );

                    return false;
                }

                Debug.WriteLine($"[Client] Steam is fully logged in for {LoginDetails.Username}");

                OnSteamStarted?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[Client] Exception while starting Steam for {LoginDetails.Username}: {ex}"
                );

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

        public void StopGMOD()
        {
            if (GMOD == null || GMOD.HasExited || Steam == null || Steam.HasExited)
                return;
            if (RCON != null && RCON.Connected)
                RCON.SendCommandAsync("quit");
            if (RCON != null)
                RCON.Dispose();
            try
            {
                GMOD.Close();
            }
            catch { }
            GMOD.Dispose();
        }

        public void SetGMOD(uint ProcessID)
        {
            GMOD = Process.GetProcessById((int)ProcessID);
        }

        public bool IsSteamOpen()
        {
            if (Steam == null || Steam.HasExited)
                return false;
            return true;
        }

        public bool IsGMODOpen()
        {
            if (GMOD == null || GMOD.HasExited)
                return false;
            return true;
        }

        public Client(LoginDetails loginDetails, Sandboxie enviroment)
        {
            LoginDetails = loginDetails;
            Enviroment = enviroment;
            ConnectedServer = null;

            Debug.WriteLine($"[Client] Creating client for {loginDetails.Username}");

            Task.Run(
                async () =>
                {
                    bool steamStarted = await InitialiseSteamAsync();
                    if (!steamStarted)
                        return;
                }
            );

            Debug.WriteLine($"[Client] Subscribed too OnSteamStarted");

            OnGMODStarted += (sender, e) =>
            {
                InitialiseClientLoop();
            };

            Debug.WriteLine($"[Client] Subscribed too OnGMODStarted");
        }

        public void Dispose()
        {
            Disconnect();
            RCON?.Dispose();
            StopGMOD();
            StopSteam();
        }
    }
}
