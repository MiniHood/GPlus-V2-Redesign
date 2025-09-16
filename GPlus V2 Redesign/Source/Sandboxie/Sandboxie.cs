using CoreRCON;
using GPlus.Game.Clients;
using GPlus.Source.Steam;
using GPlus.Source.Structs;

namespace GPlus.Source.Sandboxing
{
    internal class Sandboxie
    {
        public RCON? _rconConnection;
        public string _sandboxName;
        public Client _client;
        public SteamCMD steamCMD;

        public Sandboxie(LoginDetails loginDetails)
        {
            _sandboxName = loginDetails.Username;
            _client = ClientManager.CreateClient(loginDetails, this);
        }
    }
}
