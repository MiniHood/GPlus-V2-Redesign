using CoreRCON;
using CoreRCON.Parsers.Standard;
using GPlus_V2_Redesign.Game.Servers;
using GPlus_V2_Redesign.Source;
using GPlus_V2_Redesign.Source.Sandboxie;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8601 // Possible null reference assignment.


namespace GPlus_V2_Redesign.Game.Clients
{

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

        public bool InitialiseSteam()
        {
            SandboxieWrapper.SB_RESULT<Process> ProcessResult = SandboxieWrapper.RunBoxed(
                $"{SettingsManager.CurrentSettings.General.SteamPath} " +
                $"-login" +
                $" {LoginDetails.UncleanedUsername}" +
                $" {LoginDetails.Password}",
                $"{Enviroment._sandboxName}"
            );
            if (ProcessResult.Data == null)
                return false;
            Steam = ProcessResult.Data;
            OnSteamStarted?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public void StopSteam()
        {
            if (Steam == null || Steam.HasExited)
                return;
            Steam.Close();
            Steam.Dispose();
        }

        public bool InitialiseGMOD()
        {
            if(Steam == null)
                return false;

            SandboxieWrapper.SB_RESULT<Process> ProcessResult = SandboxieWrapper.RunBoxed(
                $"{SettingsManager.CurrentSettings.General.GMODDirectory}\\" +
                $"{SettingsManager.CurrentSettings.General.GMODExecutable} " +
                $"-{SettingsManager.CurrentSettings.General.GMODLaunchArguments}"
            );
            if (ProcessResult.Data == null)
                return false;
            GMOD = ProcessResult.Data;
            OnGMODStarted?.Invoke(this, EventArgs.Empty);
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
            while(true)
            {
                if(Steam == null)
                    break;
                
                if(GMOD == null)
                    break;

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

            InitialiseSteam();
            InitialiseGMOD();
            InitialiseClientLoop();
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
