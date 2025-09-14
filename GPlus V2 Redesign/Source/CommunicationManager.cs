using GPlus_V2_Redesign.Game.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static GPlus_V2_Redesign.Source.Memory;

namespace GPlus_V2_Redesign.Source
{
    internal static class CommunicationManager
    {
        public static void ParseCommunication(Message m)
        {
            COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
            SteamMessage msg = (SteamMessage)Marshal.PtrToStructure(cds.lpData, typeof(SteamMessage));
            ClientManager.AttemptSync(msg);       
        }

    }
}
