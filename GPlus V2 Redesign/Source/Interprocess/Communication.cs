using System.Runtime.InteropServices;
using static GPlus.Source.Interprocess.Memory;

namespace GPlus.Source.Interprocess
{
    internal static class Communication
    {
        public static void ParseCommunication(Message m)
        {
            // This was originally for syncing gmod process using WM_COPYDATA however
            // I'm going to change this to pipes aswell as using it for more in depth control
            // of the clients, however this will be done later.

            COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
            SteamMessage msg = (SteamMessage)Marshal.PtrToStructure(cds.lpData, typeof(SteamMessage));
            // ClientManager.AttemptSync(msg);
        }

    }
}
