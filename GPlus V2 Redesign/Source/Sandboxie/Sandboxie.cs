using CoreRCON;
using GPlus_V2_Redesign.Game.Clients;

namespace GPlus_V2_Redesign.Source.Sandboxie
{
    internal class Sandboxie
    {
        public RCON? _rconConnection;
        public string _sandboxName;
        public Client _client;
        public Sandboxie(LoginDetails loginDetails)
        {
            _sandboxName = loginDetails.CleanedUsername;

            // Create a client for this sandbox
            _client = new Client(loginDetails, this);
        }
    }
}
