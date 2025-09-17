using CoreRCON;
using GPlus.Game.Clients;
using GPlus.Source.Steam;
using GPlus.Source.Structs;

namespace GPlus.Source.Sandboxing
{
    internal class Sandboxie
    {
        public RCON? RconConnection { get; set; }
        public string SandboxName { get; }
        public Client? Client { get; private set; }
        public SteamCMD SteamCmd { get; } = new();

        public Sandboxie(string username)
        {
            SandboxName = username ?? throw new ArgumentNullException(nameof(username));
        }

        public async Task InitialiseAsync(LoginDetails loginDetails)
        {
            if (Client != null)
                throw new InvalidOperationException("Sandboxie is already initialised.");

            Client = await ClientManager.CreateClientAsync(loginDetails, this);
        }
    }
}
