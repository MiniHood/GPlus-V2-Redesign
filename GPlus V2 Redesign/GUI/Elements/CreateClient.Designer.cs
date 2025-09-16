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
            _txtUsername = new ReaLTaiizor.Controls.CrownTextBox();
            _txtPassword = new ReaLTaiizor.Controls.CrownTextBox();
            _lblAddClient = new ReaLTaiizor.Controls.CrownLabel();
            materialDivider1 = new ReaLTaiizor.Controls.MaterialDivider();
            materialDivider2 = new ReaLTaiizor.Controls.MaterialDivider();
            _cbSaveLogin = new ReaLTaiizor.Controls.CheckBox();
            materialDivider3 = new ReaLTaiizor.Controls.MaterialDivider();
            _btnCreateClient = new ReaLTaiizor.Controls.ParrotButton();
            _btnClose = new ReaLTaiizor.Controls.ParrotButton();
            SuspendLayout();
            // 
            // _txtUsername
            // 
            _txtUsername.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _txtUsername.BackColor = Color.FromArgb(32, 32, 32);
            _txtUsername.BorderStyle = BorderStyle.None;
            _txtUsername.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _txtUsername.ForeColor = Color.FromArgb(220, 220, 220);
            _txtUsername.Location = new Point(0, 66);
            _txtUsername.Margin = new Padding(20);
            _txtUsername.Name = "_txtUsername";
            _txtUsername.PlaceholderText = "Username";
            _txtUsername.Size = new Size(233, 22);
            _txtUsername.TabIndex = 0;
            _txtUsername.TextAlign = HorizontalAlignment.Center;
            // 
            // _txtPassword
            // 
            _txtPassword.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _txtPassword.BackColor = Color.FromArgb(32, 32, 32);
            _txtPassword.BorderStyle = BorderStyle.None;
            _txtPassword.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _txtPassword.ForeColor = Color.FromArgb(220, 220, 220);
            _txtPassword.Location = new Point(0, 104);
            _txtPassword.Margin = new Padding(20);
            _txtPassword.Name = "_txtPassword";
            _txtPassword.PlaceholderText = "Password";
            _txtPassword.Size = new Size(233, 22);
            _txtPassword.TabIndex = 1;
            _txtPassword.TextAlign = HorizontalAlignment.Center;
            _txtPassword.UseSystemPasswordChar = true;
            // 
            // _lblAddClient
            // 
            _lblAddClient.Dock = DockStyle.Top;
            _lblAddClient.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lblAddClient.ForeColor = Color.FromArgb(220, 220, 220);
            _lblAddClient.Location = new Point(0, 0);
            _lblAddClient.Name = "_lblAddClient";
            _lblAddClient.Size = new Size(233, 39);
            _lblAddClient.TabIndex = 2;
            _lblAddClient.Text = "Add Client";
            _lblAddClient.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // materialDivider1
            // 
            materialDivider1.BackColor = Color.FromArgb(30, 0, 0, 0);
            materialDivider1.Depth = 0;
            materialDivider1.Location = new Point(49, 42);
            materialDivider1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialDivider1.Name = "materialDivider1";
            materialDivider1.Size = new Size(137, 3);
            materialDivider1.TabIndex = 3;
            materialDivider1.Text = "materialDivider1";
            // 
            // materialDivider2
            // 
            materialDivider2.BackColor = Color.FromArgb(30, 0, 0, 0);
            materialDivider2.Depth = 0;
            materialDivider2.Location = new Point(50, 134);
            materialDivider2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialDivider2.Name = "materialDivider2";
            materialDivider2.Size = new Size(137, 3);
            materialDivider2.TabIndex = 4;
            materialDivider2.Text = "materialDivider2";
            // 
            // _cbSaveLogin
            // 
            _cbSaveLogin.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _cbSaveLogin.Checked = true;
            _cbSaveLogin.CheckedBackColor = Color.FromArgb(42, 42, 42);
            _cbSaveLogin.CheckedBorderColor = Color.FromArgb(46, 46, 46);
            _cbSaveLogin.CheckedDisabledColor = Color.Gray;
            _cbSaveLogin.CheckedEnabledColor = Color.Lime;
            _cbSaveLogin.Enable = true;
            _cbSaveLogin.Font = new Font("Microsoft Sans Serif", 9F);
            _cbSaveLogin.ForeColor = Color.Gainsboro;
            _cbSaveLogin.Location = new Point(59, 158);
            _cbSaveLogin.Name = "_cbSaveLogin";
            _cbSaveLogin.Size = new Size(119, 16);
            _cbSaveLogin.TabIndex = 5;
            _cbSaveLogin.Text = "Save User Login";
            // 
            // materialDivider3
            // 
            materialDivider3.BackColor = Color.FromArgb(30, 0, 0, 0);
            materialDivider3.Depth = 0;
            materialDivider3.Location = new Point(50, 178);
            materialDivider3.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialDivider3.Name = "materialDivider3";
            materialDivider3.Size = new Size(137, 3);
            materialDivider3.TabIndex = 6;
            materialDivider3.Text = "materialDivider3";
            // 
            // _btnCreateClient
            // 
            _btnCreateClient.BackgroundColor = Color.FromArgb(26, 26, 26);
            _btnCreateClient.ButtonImage = null;
            _btnCreateClient.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnCreateClient.ButtonText = "Create Client";
            _btnCreateClient.ClickBackColor = Color.FromArgb(195, 195, 195);
            _btnCreateClient.ClickTextColor = Color.FromArgb(22, 22, 22);
            _btnCreateClient.CornerRadius = 5;
            _btnCreateClient.Horizontal_Alignment = StringAlignment.Center;
            _btnCreateClient.HoverBackgroundColor = Color.FromArgb(22, 22, 22);
            _btnCreateClient.HoverTextColor = Color.Gainsboro;
            _btnCreateClient.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
            _btnCreateClient.Location = new Point(59, 193);
            _btnCreateClient.Name = "_btnCreateClient";
            _btnCreateClient.Size = new Size(123, 26);
            _btnCreateClient.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnCreateClient.TabIndex = 7;
            _btnCreateClient.TextColor = SystemColors.GrayText;
            _btnCreateClient.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnCreateClient.Vertical_Alignment = StringAlignment.Center;
            _btnCreateClient.Click += _btnCreateClient_Click;
            // 
            // _btnClose
            // 
            _btnClose.BackgroundColor = Color.FromArgb(26, 26, 26);
            _btnClose.ButtonImage = null;
            _btnClose.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnClose.ButtonText = "Close";
            _btnClose.ClickBackColor = Color.FromArgb(195, 195, 195);
            _btnClose.ClickTextColor = Color.Red;
            _btnClose.CornerRadius = 5;
            _btnClose.Horizontal_Alignment = StringAlignment.Center;
            _btnClose.HoverBackgroundColor = Color.FromArgb(22, 22, 22);
            _btnClose.HoverTextColor = Color.Red;
            _btnClose.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
            _btnClose.Location = new Point(84, 225);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new Size(72, 26);
            _btnClose.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnClose.TabIndex = 8;
            _btnClose.TextColor = Color.Firebrick;
            _btnClose.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnClose.Vertical_Alignment = StringAlignment.Center;
            _btnClose.Click += _btnClose_Click;
            // 
            // CreateClient
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            Controls.Add(_btnClose);
            Controls.Add(_btnCreateClient);
            Controls.Add(materialDivider3);
            Controls.Add(_cbSaveLogin);
            Controls.Add(materialDivider2);
            Controls.Add(materialDivider1);
            Controls.Add(_lblAddClient);
            Controls.Add(_txtPassword);
            Controls.Add(_txtUsername);
            Name = "CreateClient";
            Size = new Size(233, 262);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ReaLTaiizor.Controls.CrownTextBox _txtUsername;
        private ReaLTaiizor.Controls.CrownTextBox _txtPassword;
        private ReaLTaiizor.Controls.CrownLabel _lblAddClient;
        private ReaLTaiizor.Controls.MaterialDivider materialDivider1;
        private ReaLTaiizor.Controls.MaterialDivider materialDivider2;
        private ReaLTaiizor.Controls.CheckBox _cbSaveLogin;
        private ReaLTaiizor.Controls.MaterialDivider materialDivider3;
        private ReaLTaiizor.Controls.ParrotButton _btnCreateClient;
        private ReaLTaiizor.Controls.ParrotButton _btnClose;
    }
}
