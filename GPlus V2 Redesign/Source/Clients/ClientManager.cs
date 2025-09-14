using GPlus_V2_Redesign.GUI;
using GPlus_V2_Redesign.GUI.Helpers;
using GPlus_V2_Redesign.Source;
using GPlus_V2_Redesign.Source.Sandboxie;
using System.Diagnostics;

namespace GPlus_V2_Redesign.Game.Clients
{
    internal static class ClientManager
    {
        private static List<Client> _clients = new List<Client>();
        
        private static void RegisterClient(Client client)
        {
            _clients.Add(client);
            UserControlLoader.Clients?.RefreshClientList();
        }

        public static void UnregisterClient(Client client)
        {
            client.StopGMOD();
            client.StopSteam();
            client.ConnectedServer = null;
            _clients.Remove(client);
        }

        public static Client CreateClient(LoginDetails login, Sandboxie Enviroment)
        {
            Client client = new Client(login, Enviroment);
            RegisterClient(client);
            return client;
        }

        private static void ScanSyncOpenGMOD()
        {
            // Okay this is weird, we're going to have to scan every open gmod process in all sandboxes, then with our dll request the proc id and steam id, then sync them up in a list that way. 
        }

        public static List<Client> GetAllClients()
        {
            return _clients;
        }
    }
}
