namespace GPlus.GUI.Elements
{
    public partial class Clients : UserControl
    {
        public Clients()
        {
            InitializeComponent();
        }

        public void RefreshClientList()
        {
            var clients = Game.Clients.ClientManager.GetAllClients();
            _listClients.Items.Clear();
            _listClients.Groups.Clear();

            // Group clients by server IP
            var grouped = clients
                .GroupBy(c => c.ConnectedServer?.IP ?? "No IP")
                .ToList();

            foreach (var group in grouped)
            {
                // Create a ListViewGroup with the server name as display
                string serverName = group.First().ConnectedServer?.Name ?? "Not Connected";
                ListViewGroup listGroup = new ListViewGroup(serverName, HorizontalAlignment.Left);
                _listClients.Groups.Add(listGroup);

                // Add clients to the group
                foreach (var client in group)
                {
                    var item = new ListViewItem(
                        new[] { client.LoginDetails.Username, client.ConnectedServer?.Name ?? "Not Connected", client.Enviroment._sandboxName },
                        listGroup
                    );
                    item.Tag = client;
                    _listClients.Items.Add(item);
                }
            }
        }

        private void Clients_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            RefreshClientList();
        }

        private void _btnCreateClient_Click(object sender, EventArgs e)
        {
            _ucCreateClient.BringToFront();
            _ucCreateClient.Show();
        }

        private void _ucCreateClient_Load(object sender, EventArgs e)
        {

        }
    }
}
