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

namespace GPlus.Game.Clients
{
    internal static class ClientManager
    {
        private static readonly List<Client> _clients = new();

        private static async Task<bool> HasTwoFactorAuthAsync(Client client)
        {
            var response = await SteamCMD.DoesClientHave2FA(
                client.LoginDetails,
                sandboxed: true,
                sandbox: client.Environment
            );

            if (response.response == ClientResponse.AUTHENABLED)
            {
                SandboxieManager.DeleteSandbox(client.Environment);
                Debug.WriteLine($"[Client] {client.LoginDetails.Username} has 2FA enabled, cannot continue.");
                return true;
            }

            return false;
        }

        private static void RegisterClient(Client client)
        {
            _clients.Add(client);
            UserControlLoader.Clients?.RefreshClientList();
        }

        public static void UnregisterClient(Client client)
        {
            client.Dispose();
            _clients.Remove(client);
            UserControlLoader.Clients?.RefreshClientList();
        }

        public static async Task<Client?> CreateClientAsync(LoginDetails login, Sandboxie environment)
        {
            var client = new Client(login, environment);

            if (await HasTwoFactorAuthAsync(client))
                return null;

            RegisterClient(client);
            return client;
        }

        public static async Task AttemptSyncAsync(SteamMessage msg)
        {
            Debug.WriteLine($"[Client] Attempting to sync {msg.steamID} with PID {msg.processID}");

            // Match SteamID to username
            var username = await Steam.GetSteamUsernameAsync(msg.steamID.ToString());
            if (string.IsNullOrEmpty(username))
            {
                Debug.WriteLine("[Client] Failed to get username from SteamID.");
                return;
            }

            var client = GetClientByUsername(username);
            if (client == null)
            {
                Debug.WriteLine($"[Client] No client found for username {username}.");
                return;
            }

            client.SetGMOD(msg.processID);
            Debug.WriteLine($"[Client] Synced {username} with GMOD process {msg.processID}.");
        }

        public static IReadOnlyList<Client> GetAllClients() => _clients;

        public static Client? GetClientByUsername(string username) =>
            _clients.FirstOrDefault(c =>
                string.Equals(c.LoginDetails.Username, username, StringComparison.OrdinalIgnoreCase));
    }
}
