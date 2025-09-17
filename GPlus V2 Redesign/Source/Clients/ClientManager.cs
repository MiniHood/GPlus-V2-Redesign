using GPlus.GUI.Helpers;
using GPlus.Source.Enums;
using GPlus.Source.Sandboxing;
using GPlus.Source.Steam;
using GPlus.Source.Structs;
using System.Diagnostics;

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

        private async static Task RegisterClient(Client client)
        {
            _clients.Add(client);
            await UserControlLoader.Clients?.RefreshClientListAsync();
            await SaveClients();
        }

        public async static Task UnregisterClient(Client client)
        {
            client.Dispose();
            _clients.Remove(client);
            await UserControlLoader.Clients?.RefreshClientListAsync();
        }

        public async static Task OnShutdown()
        {
            foreach (var client in _clients.ToList())
                await UnregisterClient(client);
        }

        public static async Task<Client?> CreateClientAsync(LoginDetails login, Sandboxie environment)
        {
            var client = new Client(login, environment);

            if (await HasTwoFactorAuthAsync(client))
                return null;

            await RegisterClient(client);
            return client;
        }

        public static async Task LoadSavedClients()
        {
            if (!File.Exists(ClientsPath))
                return;

            var encrypted = await File.ReadAllTextAsync(ClientsPath);
            var decrypted = FileProtection.Unprotect(encrypted);
            var savedlogins = JsonConvert.DeserializeObject<List<LoginDetails>>(decrypted, new JsonSerializerSettings { Formatting = Formatting.Indented });
            if (savedlogins == null) return;
            foreach (var login in savedlogins)
            {
                await SandboxieManager.CreateNewSandboxAsync(login);
            }
        }

        public static async Task SaveClients()
        {
            var logins = _clients.Select(c => c.LoginDetails).ToList();
            var json = JsonConvert.SerializeObject(logins, Formatting.Indented);
            var encrypted = FileProtection.Protect(json);
            await File.WriteAllTextAsync(ClientsPath, encrypted);
        }

        public static IReadOnlyList<Client> GetAllClients() => _clients;

        public static Client? GetClientByUsername(string username) =>
            _clients.FirstOrDefault(c =>
                string.Equals(c.LoginDetails.Username, username, StringComparison.OrdinalIgnoreCase));
    }
}
