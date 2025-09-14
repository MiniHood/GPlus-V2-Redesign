using GPlus_V2_Redesign.Game.Clients;
using GPlus_V2_Redesign.Source.Network;

namespace GPlus_V2_Redesign.Source.Sandboxie
{
    internal class Sandboxie
    {
        public RCON _rconConnection;
        public string _sandboxName;
        public Client _client;
        public Sandboxie(string sBoxName, RCON Connection)
        {
            _sandboxName = sBoxName;
            _rconConnection = Connection;
        }
    }
}
