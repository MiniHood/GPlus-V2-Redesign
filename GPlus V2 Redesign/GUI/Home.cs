using GPlus.GUI.Elements;
using GPlus.GUI.Helpers;
using GPlus.Source;
using GPlus.Source.Interprocess;
using GPlus.Source.Steam;

namespace GPlus
{
    public partial class Home : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public static Home? Instance { get; private set; }

        public static bool FormShutdownAllowed = false;

        public Home()
        {
            InitializeComponent();
            Instance = this;
        }

        public static void LoadUserControls()
        {
            UserControlLoader.InitializeUserControls(
                Instance._ucDashboard,
                Instance._ucClients,
                Instance._ucServers,
                Instance._ucSettings,
                Instance._ucNavBar,
                Instance._ucShuttingDown
            ); // _ucSetup isn't needed.
        }

        private void Home_Load(object sender, EventArgs e)
        {
            if (DesignMode) return; // why microsoft... Why?
        }

        private void Home_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private async void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormShutdownAllowed)
            {
                e.Cancel = true;
                await ShuttingDown.OnProcessExit();
            }
            return;
        }
    }
}
