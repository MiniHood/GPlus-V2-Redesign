using GPlus.Game.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static GPlus.Source.Interprocess.Memory;

namespace GPlus.Source.Interprocess
{
    internal static class Communication
    {
        public static void ParseCommunication(Message m)
        {
            COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
            SteamMessage msg = (SteamMessage)Marshal.PtrToStructure(cds.lpData, typeof(SteamMessage));
            ClientManager.AttemptSync(msg);       
        }

    }
}
