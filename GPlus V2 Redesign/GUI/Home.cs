using GPlus.GUI.Helpers;
using GPlus.Source;
using GPlus.Source.Interprocess;
using System.Configuration;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using static GPlus.Source.Interprocess.Memory;

namespace GPlus
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
                Communication.ParseCommunication(m);
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
