using GPlus.Game.Clients;
using GPlus.GUI.Elements.Client.Popups;

namespace GPlus.GUI.Elements
{
    public partial class Clients : UserControl
    {
        public Clients()
        {
            InitializeComponent();
        }

        public async Task RefreshClientListAsync()
        {
            // Capture client list on background thread
            var clients = await Task.Run(() =>
            {
                return Game.Clients.ClientManager
                    .GetAllClients()
                    .GroupBy(c => c.ConnectedServer?.IP ?? "No IP")
                    .Select(group => new
                    {
                        ServerName = group.First().ConnectedServer?.Name ?? "Not Connected",
                        Clients = group.ToList()
                    })
                    .ToList();
            });

            // Back on UI thread, update ListView
            if (_listClients.InvokeRequired)
            {
                _listClients.Invoke(() => UpdateListView(clients));
            }
            else
            {
                UpdateListView(clients);
            }
        }

        private void UpdateListView(IEnumerable<dynamic> grouped)
        {
            _listClients.BeginUpdate();
            _listClients.Items.Clear();
            _listClients.Groups.Clear();

            foreach (var group in grouped)
            {
                var listGroup = new ListViewGroup(group.ServerName, HorizontalAlignment.Left);
                _listClients.Groups.Add(listGroup);

                foreach (var client in group.Clients)
                {
                    var item = new ListViewItem(
                        new string[]
                        {
                            client.LoginDetails.Username,
                            client.ConnectedServer?.Name ?? "Not Connected",
                            client.Environment.SandboxName
                        },
                        listGroup
                    );

                    item.Tag = client;
                    _listClients.Items.Add(item);
                }
            }

            _listClients.EndUpdate();
        }


        private void Clients_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            Task.Run(async () =>
            {
                await RefreshClientListAsync();
            });
        }

        private void _btnCreateClient_Click(object sender, EventArgs e)
        {
            _ucCreateClient.BringToFront();
            _ucCreateClient.Show();
        }

        private void UpdateClientControlsUI()
        {
            if (_ucClientControls.InvokeRequired)
            {
                _ucClientControls.Invoke(() => _ucClientControls.RefreshUI());
            }
            else
            {
                _ucClientControls.RefreshUI();
            }
        }

        private void _listClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listClients.SelectedItems.Count == 0)
                return;

            var selectedItem = _listClients.SelectedItems[0];
            if (selectedItem.Tag is Game.Clients.Client client)
            {
                ClientControls.SelectedClient = client;
                UpdateClientControlsUI();
            }
        }
    }
}
