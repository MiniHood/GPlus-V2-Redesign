using GPlus.GUI.Helpers;

namespace GPlus.GUI.Elements
{
    public partial class NavBar : UserControl
    {
        public NavBar()
        {
            InitializeComponent();
        }

        private void _btnDashboard_Click(object sender, EventArgs e)
        {
            UserControlLoader.Dashboard?.BringToFront();
        }

        private void _btnServers_Click(object sender, EventArgs e)
        {
            UserControlLoader.Servers?.BringToFront();
        }

        private void _btnClients_Click(object sender, EventArgs e)
        {
            UserControlLoader.Clients?.BringToFront();
        }

        private void _btnSettings_Click(object sender, EventArgs e)
        {
            UserControlLoader.Settings?.BringToFront();
        }
    }
}
