using GPlus_V2_Redesign.GUI.Helpers;

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
            UserControlLoader.InitializeUserControls(_ucDashboard, _ucClients, _ucServers, _ucSettings, _ucNavBar);
        }
    }
}
