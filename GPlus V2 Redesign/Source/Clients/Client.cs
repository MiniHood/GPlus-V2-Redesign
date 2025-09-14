using GPlus_V2_Redesign.Game.Servers;
using GPlus_V2_Redesign.Source;
using GPlus_V2_Redesign.Source.Network;
using GPlus_V2_Redesign.Source.Sandboxie;

namespace GPlus_V2_Redesign.Game.Clients
{
    internal class Client
    {
        public LoginDetails LoginDetails;
        public Server? ConnectedServer;
        public required Sandboxie Enviroment;
        public bool IsConnected => ConnectedServer != null;

        private async Task ClientLoop()
        {
            while (true) {
                // Constantly check status on connection
                Enviroment._rconConnection

                await Task.Delay(100);
            }
        }

        public Client(LoginDetails loginDetails)
        {
            LoginDetails = loginDetails;
            Enviroment = SandboxieManager.CreateNewSandbox(LoginDetails);
            ConnectedServer = null;
        }

    }
}
