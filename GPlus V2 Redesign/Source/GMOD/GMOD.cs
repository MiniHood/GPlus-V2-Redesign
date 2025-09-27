using GPlus.Source.Enums;
using GPlus.Source.Interprocess;
using GPlus.Source.Structs;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;

namespace GPlus.Source.GMOD
{
    internal class GMOD
    {
        public Process Process { get; set; }
        public static GMOD Instance;
        public GMOD() => Instance = this;
        public bool LuaReady { get; set; } = false;

        internal async Task SendPrintTest()
        {
            await CommunicationTCP.SendCommandToClient(Process.Id, "LUA", "print(\"Testing\")");
        }
    }
}
