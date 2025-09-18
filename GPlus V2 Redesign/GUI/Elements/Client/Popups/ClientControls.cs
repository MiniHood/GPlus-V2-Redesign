using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GPlus.Game.Clients;
namespace GPlus.GUI.Elements.Client.Popups
{
    public partial class ClientControls : UserControl
    {
        internal GPlus.Game.Clients.Client SelectedClient;
        public ClientControls()
        {
            InitializeComponent();
        }

        private void ClientControls_Load(object sender, EventArgs e)
        {
            
        }
    }
}
