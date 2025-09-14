using GPlus_V2_Redesign.GUI.Helpers;
using GPlus_V2_Redesign.Source;
using System.Configuration;

namespace GPlus_V2_Redesign
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            SettingsManager.LoadSettings();
            UserControlLoader.InitializeUserControls(_ucDashboard, _ucClients, _ucServers, _ucSettings, _ucNavBar);
        }
    }
}
