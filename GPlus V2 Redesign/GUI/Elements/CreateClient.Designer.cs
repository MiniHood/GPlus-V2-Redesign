namespace GPlus.GUI.Elements
{
    partial class CreateClient
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _txtUsername = new ReaLTaiizor.Controls.NightTextBox();
            _txtPassword = new ReaLTaiizor.Controls.NightTextBox();
            _btnAddClient = new ReaLTaiizor.Controls.Button();
            SuspendLayout();
            // 
            // _txtUsername
            // 
            _txtUsername.ActiveBorderColor = Color.FromArgb(242, 93, 89);
            _txtUsername.BackColor = Color.FromArgb(32, 32, 32);
            _txtUsername.BaseBackColor = Color.FromArgb(32, 32, 32);
            _txtUsername.ColorBordersOnEnter = true;
            _txtUsername.DisableBorderColor = Color.FromArgb(36, 36, 36);
            _txtUsername.Dock = DockStyle.Top;
            _txtUsername.Font = new Font("Segoe UI", 10F);
            _txtUsername.ForeColor = Color.White;
            _txtUsername.Image = null;
            _txtUsername.Location = new Point(0, 0);
            _txtUsername.MaxLength = 32767;
            _txtUsername.Multiline = false;
            _txtUsername.Name = "_txtUsername";
            _txtUsername.ReadOnly = false;
            _txtUsername.ShortcutsEnabled = true;
            _txtUsername.ShowBottomBorder = true;
            _txtUsername.ShowTopBorder = true;
            _txtUsername.Size = new Size(394, 50);
            _txtUsername.TabIndex = 0;
            _txtUsername.Text = "Username";
            _txtUsername.TextAlignment = HorizontalAlignment.Left;
            _txtUsername.UseSystemPasswordChar = false;
            _txtUsername.Watermark = "";
            _txtUsername.WatermarkColor = Color.FromArgb(116, 120, 129);
            // 
            // _txtPassword
            // 
            _txtPassword.ActiveBorderColor = Color.FromArgb(242, 93, 89);
            _txtPassword.BackColor = Color.FromArgb(32, 32, 32);
            _txtPassword.BaseBackColor = Color.FromArgb(32, 32, 32);
            _txtPassword.ColorBordersOnEnter = true;
            _txtPassword.DisableBorderColor = Color.FromArgb(36, 36, 36);
            _txtPassword.Dock = DockStyle.Top;
            _txtPassword.Font = new Font("Segoe UI", 10F);
            _txtPassword.ForeColor = Color.White;
            _txtPassword.Image = null;
            _txtPassword.Location = new Point(0, 50);
            _txtPassword.MaxLength = 32767;
            _txtPassword.Multiline = false;
            _txtPassword.Name = "_txtPassword";
            _txtPassword.ReadOnly = false;
            _txtPassword.ShortcutsEnabled = true;
            _txtPassword.ShowBottomBorder = true;
            _txtPassword.ShowTopBorder = true;
            _txtPassword.Size = new Size(394, 50);
            _txtPassword.TabIndex = 1;
            _txtPassword.Text = "Password";
            _txtPassword.TextAlignment = HorizontalAlignment.Left;
            _txtPassword.UseSystemPasswordChar = false;
            _txtPassword.Watermark = "";
            _txtPassword.WatermarkColor = Color.FromArgb(116, 120, 129);
            // 
            // _btnAddClient
            // 
            _btnAddClient.BackColor = Color.Transparent;
            _btnAddClient.BorderColor = Color.FromArgb(32, 34, 37);
            _btnAddClient.Dock = DockStyle.Top;
            _btnAddClient.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            _btnAddClient.EnteredColor = Color.FromArgb(32, 34, 37);
            _btnAddClient.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _btnAddClient.Image = null;
            _btnAddClient.ImageAlign = ContentAlignment.MiddleLeft;
            _btnAddClient.InactiveColor = Color.FromArgb(32, 34, 37);
            _btnAddClient.Location = new Point(0, 100);
            _btnAddClient.Name = "_btnAddClient";
            _btnAddClient.PressedBorderColor = Color.FromArgb(45, 45, 45);
            _btnAddClient.PressedColor = Color.FromArgb(36, 36, 36);
            _btnAddClient.Size = new Size(394, 57);
            _btnAddClient.TabIndex = 7;
            _btnAddClient.Text = "Add Client";
            _btnAddClient.TextAlignment = StringAlignment.Center;
            _btnAddClient.Click += _btnAddClient_Click;
            // 
            // CreateClient
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            Controls.Add(_btnAddClient);
            Controls.Add(_txtPassword);
            Controls.Add(_txtUsername);
            Name = "CreateClient";
            Size = new Size(394, 157);
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.NightTextBox _txtUsername;
        private ReaLTaiizor.Controls.NightTextBox _txtPassword;
        private ReaLTaiizor.Controls.Button _btnAddClient;
    }
}
