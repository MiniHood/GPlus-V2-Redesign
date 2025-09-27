using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GPlus.Game.Clients;
using GPlus.Source.Interprocess;
namespace GPlus.GUI.Elements.Client.Popups
{
    public partial class ClientControls : UserControl
    {
        internal static Game.Clients.Client SelectedClient;
        public ClientControls()
        {
            InitializeComponent();
        }

        internal async void RefreshUI()
        {
            if (SelectedClient == null) return;

            // Update labels, textboxes, etc.
            _lblUsername.Text = SelectedClient.LoginDetails.Username;
            if(SelectedClient.GMOD.Process == null)
            {
                _lblProcessID.Text = $"Process ID: Starting...";
                _lblOnWebsocket.Text = $"Is Lua Ready: {(SelectedClient.GMOD.LuaReady ? "Yes" : "No")}";
            } else
            {
                _lblProcessID.Text = $"Process ID: {SelectedClient.GMOD.Process.Id.ToString()}";
                _lblOnWebsocket.Text = $"Is Lua Ready: {(SelectedClient.GMOD.LuaReady ? "Yes" : "No")}";
            }
        }

        private void ClientControls_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

        }

        private async void _btnTestLua_Click(object sender, EventArgs e)
        {
            if (SelectedClient == null)
                return;

            await SelectedClient.GMOD.SendPrintTest();
        }
    }
}
