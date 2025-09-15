namespace GPlus.Game.Servers
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
