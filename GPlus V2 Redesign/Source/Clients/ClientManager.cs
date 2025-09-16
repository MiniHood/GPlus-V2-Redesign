using GPlus.GUI.Elements;
using GPlus.GUI.Helpers;
using GPlus.Source.Enums;
using GPlus.Source.Interprocess;
using GPlus.Source.Network;
using GPlus.Source.Sandboxing;
using GPlus.Source.Steam;
using GPlus.Source.Structs;
using System.Diagnostics;
using static GPlus.Source.Interprocess.Memory;

#pragma warning disable CS8603 // Possible null reference return.

namespace GPlus.Game.Clients
{
    internal static class ClientManager
    {
        private static List<Client> _clients = new List<Client>();

        private static async Task<bool> Check2FA(Client client)
        {
            ClientResponse response = await SteamCMD.DoesClientHave2FA(
                client.LoginDetails,
                Sandboxed: true,
                _sandbox: client.Enviroment
            );

            if (response == ClientResponse.AUTHENABLED)
            {
                SandboxieManager.DeleteSandbox(client.Enviroment);
                Debug.WriteLine($"[Client] Client {client.LoginDetails.Username} has 2FA enabled, cannot continue.");
                return true;
            }

            return false;
        }


        /*public static async void InitializingClientManager()
        {
            await Task.Run(async () => { while (true) { ScanSyncOpenGMOD(); await Task.Delay(500); } });
        }*/

        private static void RegisterClient(Client client)
        {
            _clients.Add(client);
            UserControlLoader.Clients?.RefreshClientList();
        }

        public static void UnregisterClient(Client client)
        {
            client.Dispose();
            _clients.Remove(client);
        }

        public static Client CreateClient(LoginDetails login, Sandboxie Enviroment)
        {
            bool Has2FA = false;
            Client client = new Client(login, Enviroment);
            Task.Run(async () =>
            {
                if (await Check2FA(client))
                    Has2FA = true;
            });
            if (Has2FA)
                return null;
            RegisterClient(client);
            return client;
        }

        public static async void AttemptSync(SteamMessage msg)
        {
            ulong SteamID64 = msg.steamID;
            uint ProcessID = msg.processID;


            Debug.WriteLine($"[Client] Attempting to sync {SteamID64} with {ProcessID}");


            // Let's get the username of the player and match it
            string? Username = await Steam.GetSteamUsernameAsync(SteamID64.ToString());
            if (Username == null)
            {
                Debug.WriteLine($"[Client] Failed to get username.");
                return;
            }

            Client ConnectedClient = GetClientByUsername(Username);

            if (ConnectedClient == null) // This account has nothing to do with us
            {
                Debug.WriteLine($"[Client] Failed to get client by username.");
                return;
            }

            ConnectedClient.SetGMOD(ProcessID);
        }


        /*private static void ScanSyncOpenGMOD() // to be removed
        {
            // Okay this is weird, we're going to have to scan every open gmod process in all sandboxes, then check if the comms dll has been loaded, if not then it's not synced. and we sync it
            // communication is dealt with in Home.cs

            var Procs = Process.GetProcessesByName("gmod");
            if (Procs.Length == 0)
                return;

            foreach (var proc in Procs)
            {
                try
                {
                    if (Memory.IsModuleLoaded(proc.Id, "Communication.dll"))
                    {

                        Debug.WriteLine($"[Client] Module already loaded in {proc.Id}, skipping.");

                        return;
                    }


                    Debug.WriteLine($"[Client] Loading communication DLL into {proc.Id}");

                    Memory.InjectDLL(proc.Id, Application.StartupPath + "\\Resources\\Communication.dll");
                }
                catch { }
            }
        }*/

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
