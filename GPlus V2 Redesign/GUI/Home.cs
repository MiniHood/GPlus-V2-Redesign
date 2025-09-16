using GPlus.GUI.Helpers;
using GPlus.Source;
using GPlus.Source.Interprocess;
using GPlus.Source.Steam;

namespace GPlus
{
    public partial class Home : Form
    {
        public static Home? Instance { get; private set; }

        public Home()
        {
            InitializeComponent();
            Instance = this;
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

        public static void LoadUserControls()
        {
            UserControlLoader.InitializeUserControls(
                Instance._ucDashboard,
                Instance._ucClients,
                Instance._ucServers,
                Instance._ucSettings,
                Instance._ucNavBar
            ); // _ucSetup isn't needed.
        }

        private void Home_Load(object sender, EventArgs e)
        {
            if (DesignMode) return; // why microsoft... Why?            
        }
    }
}
