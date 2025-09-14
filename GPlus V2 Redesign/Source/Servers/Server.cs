using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPlus_V2_Redesign.Game.Servers
{
    internal class Server
    {
        public string IP;
        public int Port;
        public string Name;

        public Server(string ip, int port, string name)
        {
            IP = ip;
            Port = port;
            Name = name;
        }
    }
}
