using GPlus.Game.Clients;
using System.Runtime.InteropServices;
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
