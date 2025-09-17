using GPlus.GUI.Helpers;
using GPlus.Source.Enums;
using GPlus.Source.General;
using GPlus.Source.Sandboxing;
using GPlus.Source.Steam;
using GPlus.Source.Structs;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace GPlus.Game.Clients
{
    internal static class ClientManager
    {
        private static readonly List<Client> _clients = new();
        private static readonly string ClientsPath = $"{Application.StartupPath}Settings\\SavedClients.gplus";
        public static IReadOnlyList<Client> GetAllClients() => _clients;

        private static async Task<bool> HasTwoFactorAuthAsync(Client client)
        {
            var response = await SteamCMD.DoesClientHave2FA(
                client.LoginDetails
            );

            Debug.WriteLine("Response: " + response.ToString());

            switch(response.response)
            {
                case ClientResponse.AUTHENABLED:
                    await SandboxieManager.DeleteSandbox(client.Environment);
                    return true;
                case ClientResponse.INVALIDPASSWORD:
                    await SandboxieManager.DeleteSandbox(client.Environment);
                    return true;
                default:
                    return false;
            }
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
            {
                Debug.WriteLine("[CreateClientAsync] Client has 2AUTH");
                return null;
            }

            await client.InitialiseSteamAsync();

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

        public static Client? GetClientByUsername(string username) =>
            _clients.FirstOrDefault(c =>
                string.Equals(c.LoginDetails.Username, username, StringComparison.OrdinalIgnoreCase));
    }
}
