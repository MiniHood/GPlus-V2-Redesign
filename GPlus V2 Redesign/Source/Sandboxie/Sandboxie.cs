using CoreRCON;
using GPlus.Game.Clients;
using GPlus.Source.Structs;

namespace GPlus.Source.Sandboxie
{
    internal class Sandboxie
    {
        public RCON? _rconConnection;
        public string _sandboxName;
        public Client _client;
        public Sandboxie(LoginDetails loginDetails)
        {
            _sandboxName = loginDetails.Username;
            _client = ClientManager.CreateClient(loginDetails, this);
        }
    }
}
