using GPlus_V2_Redesign.GUI.Helpers;
using GPlus_V2_Redesign.Source;
using System.Configuration;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using static GPlus_V2_Redesign.Source.Memory;

namespace GPlus_V2_Redesign
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }


        protected override void WndProc(ref Message m)
        {
            const int WM_COPYDATA = 0x4A;
            if (m.Msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                SteamMessage msg = (SteamMessage)Marshal.PtrToStructure(cds.lpData, typeof(SteamMessage));
                Debug.WriteLine($"SteamID: {msg.steamID}, PID: {msg.processID}");
            }
            base.WndProc(ref m);
        }

        private void Home_Load(object sender, EventArgs e)
        {
            SettingsManager.LoadSettings();
            UserControlLoader.InitializeUserControls(_ucDashboard, _ucClients, _ucServers, _ucSettings, _ucNavBar);
        }

        private void _ucSettings_Load(object sender, EventArgs e)
        {

        }
    }
}
