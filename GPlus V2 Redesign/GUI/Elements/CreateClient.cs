using GPlus.Source.Sandboxie;
using GPlus.Source.Structs;

namespace GPlus.GUI.Elements
{
    public partial class CreateClient : UserControl
    {
        public CreateClient()
        {
            InitializeComponent();
        }

        private void _btnCreateClient_Click(object sender, EventArgs e)
        {
            SendToBack();
            Hide();

            SandboxieManager.CreateNewSandbox(new LoginDetails { Username = _txtUsername.Text, Password = _txtPassword.Text });

            _txtPassword.Text = "";
            _txtUsername.Text = "";
        }

        private void _btnClose_Click(object sender, EventArgs e)
        {
            _txtPassword.Text = "";
            _txtUsername.Text = "";
            SendToBack();
            Hide();
        }
    }
}
