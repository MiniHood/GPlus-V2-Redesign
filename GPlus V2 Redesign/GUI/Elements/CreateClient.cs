using GPlus_V2_Redesign.Game.Clients;
using GPlus_V2_Redesign.GUI.Helpers;
using GPlus_V2_Redesign.Source.Sandboxie;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPlus_V2_Redesign.GUI.Elements
{
    public partial class CreateClient : UserControl
    {
        public CreateClient()
        {
            InitializeComponent();
        }

        private void _btnAddClient_Click(object sender, EventArgs e)
        {
            SendToBack();
            Hide();

            SandboxieManager.CreateNewSandbox(new Source.LoginDetails { Username = _txtUsername.Text, Password = _txtPassword.Text });

            _txtPassword.Text = "Password";
            _txtUsername.Text = "Username";
        }
    }
}
