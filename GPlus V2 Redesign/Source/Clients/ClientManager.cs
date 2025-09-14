using GPlus_V2_Redesign.GUI;
using GPlus_V2_Redesign.GUI.Helpers;
using GPlus_V2_Redesign.Source;
using GPlus_V2_Redesign.Source.Sandboxie;
using System.Diagnostics;
using static GPlus_V2_Redesign.Source.Memory;

#pragma warning disable CS8603 // Possible null reference return.

namespace GPlus_V2_Redesign.Game.Clients
{
    internal static class ClientManager
    {
        private static List<Client> _clients = new List<Client>();
        
        public static async void InitializingClientManager()
        {
            await Task.Run(async () => { while (true) { ScanSyncOpenGMOD(); await Task.Delay(500); } } );
        }

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

        public static async void AttemptSync(SteamMessage msg)
        {
            ulong SteamID64 = msg.steamID;
            uint ProcessID = msg.processID;

#if DEBUG
            Debug.WriteLine($"[Client] Attempting to sync {SteamID64} with {ProcessID}");
#endif

            // Let's get the username of the player and match it
            string? Username = await Steam.GetSteamUsernameAsync(SteamID64.ToString());
            if (Username == null)
            {
#if DEBUG
                Debug.WriteLine($"[Client] Failed to get username.");
#endif
                return;
            }

            Client ConnectedClient = GetClientByUsername(Username);

            if (ConnectedClient == null) // This account has nothing to do with us
            {
#if DEBUG
                Debug.WriteLine($"[Client] Failed to get client by username.");
#endif
                return;
            }

            ConnectedClient.SetGMOD(ProcessID);
        }
        

        private static void ScanSyncOpenGMOD()
        {
            // Okay this is weird, we're going to have to scan every open gmod process in all sandboxes, then check if the comms dll has been loaded, if not then it's not synced. and we sync it
            // communication is dealt with in Home.cs

            var Procs = Process.GetProcessesByName("gmod");
            if (Procs.Length == 0)
                return;

            foreach (var proc in Procs) { 
                try
                {
                    if (Memory.IsModuleLoaded(proc.Id, "Communication.dll"))
                    {
#if DEBUG
                        Debug.WriteLine($"[Client] Module already loaded in {proc.Id}, skipping.");
#endif
                        return;
                    }

#if DEBUG
                    Debug.WriteLine($"[Client] Loading communication DLL into {proc.Id}");
#endif
                    Memory.InjectDLL(proc.Id, Application.StartupPath + "\\Resources\\Communication.dll");
                } catch { }
            }
        }

        public static List<Client> GetAllClients()
        {
            return _clients;
        }

        public static Client GetClientByUsername(string username)
        {
            return _clients.FirstOrDefault(c =>
                string.Equals(c.LoginDetails.Username, username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
