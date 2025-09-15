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

        private void _btnAddClient_Click(object sender, EventArgs e)
        {
            SendToBack();
            Hide();

            SandboxieManager.CreateNewSandbox(new LoginDetails { Username = _txtUsername.Text, Password = _txtPassword.Text });

            _txtPassword.Text = "Password";
            _txtUsername.Text = "Username";
        }
    }
}
