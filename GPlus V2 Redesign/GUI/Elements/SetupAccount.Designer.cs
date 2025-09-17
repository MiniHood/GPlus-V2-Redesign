namespace GPlus.GUI.Elements
{
    partial class SetupAccount
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
            components = new System.ComponentModel.Container();
            _lblTitle = new ReaLTaiizor.Controls.CrownLabel();
            _lblDisclaimer = new ReaLTaiizor.Controls.CrownLabel();
            _lblDisclaimer2 = new ReaLTaiizor.Controls.CrownLabel();
            _txtPassword = new ReaLTaiizor.Controls.CrownTextBox();
            _txtUsername = new ReaLTaiizor.Controls.CrownTextBox();
            _btnContinue = new ReaLTaiizor.Controls.ParrotButton();
            _btnGithub = new ReaLTaiizor.Controls.ParrotButton();
            _spinnerFeedback = new ReaLTaiizor.Controls.PoisonProgressSpinner();
            _ucDownloadGMOD = new DownloadGMOD();
            _tmTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = DockStyle.Top;
            _lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lblTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblTitle.Location = new Point(0, 0);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(752, 39);
            _lblTitle.TabIndex = 3;
            _lblTitle.Text = "Further action required.";
            _lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblDisclaimer
            // 
            _lblDisclaimer.Dock = DockStyle.Top;
            _lblDisclaimer.ForeColor = Color.FromArgb(220, 220, 220);
            _lblDisclaimer.Location = new Point(0, 39);
            _lblDisclaimer.Name = "_lblDisclaimer";
            _lblDisclaimer.Size = new Size(752, 15);
            _lblDisclaimer.TabIndex = 4;
            _lblDisclaimer.Text = "Please enter the details of an account with Garry's Mod for the initial download.";
            _lblDisclaimer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblDisclaimer2
            // 
            _lblDisclaimer2.Dock = DockStyle.Top;
            _lblDisclaimer2.ForeColor = Color.FromArgb(220, 220, 220);
            _lblDisclaimer2.Location = new Point(0, 54);
            _lblDisclaimer2.Name = "_lblDisclaimer2";
            _lblDisclaimer2.Size = new Size(752, 35);
            _lblDisclaimer2.TabIndex = 5;
            _lblDisclaimer2.Text = "If you are unsure of this, feel free to compile the source from the github. \r\nAbsolutely no data is sent to Plus Studios.";
            _lblDisclaimer2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _txtPassword
            // 
            _txtPassword.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _txtPassword.BackColor = Color.FromArgb(26, 26, 26);
            _txtPassword.BorderStyle = BorderStyle.None;
            _txtPassword.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _txtPassword.ForeColor = Color.FromArgb(220, 220, 220);
            _txtPassword.Location = new Point(186, 235);
            _txtPassword.Margin = new Padding(20);
            _txtPassword.Name = "_txtPassword";
            _txtPassword.PlaceholderText = "Password";
            _txtPassword.Size = new Size(386, 22);
            _txtPassword.TabIndex = 7;
            _txtPassword.TextAlign = HorizontalAlignment.Center;
            _txtPassword.UseSystemPasswordChar = true;
            // 
            // _txtUsername
            // 
            _txtUsername.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _txtUsername.BackColor = Color.FromArgb(26, 26, 26);
            _txtUsername.BorderStyle = BorderStyle.None;
            _txtUsername.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _txtUsername.ForeColor = Color.FromArgb(220, 220, 220);
            _txtUsername.Location = new Point(186, 197);
            _txtUsername.Margin = new Padding(20);
            _txtUsername.Name = "_txtUsername";
            _txtUsername.PlaceholderText = "Username";
            _txtUsername.Size = new Size(386, 22);
            _txtUsername.TabIndex = 6;
            _txtUsername.TextAlign = HorizontalAlignment.Center;
            // 
            // _btnContinue
            // 
            _btnContinue.BackgroundColor = Color.FromArgb(32, 32, 32);
            _btnContinue.ButtonImage = null;
            _btnContinue.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnContinue.ButtonText = "Continue";
            _btnContinue.ClickBackColor = Color.FromArgb(195, 195, 195);
            _btnContinue.ClickTextColor = Color.FromArgb(26, 26, 26);
            _btnContinue.CornerRadius = 5;
            _btnContinue.Horizontal_Alignment = StringAlignment.Center;
            _btnContinue.HoverBackgroundColor = Color.FromArgb(26, 26, 26);
            _btnContinue.HoverTextColor = Color.Gainsboro;
            _btnContinue.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
            _btnContinue.Location = new Point(318, 280);
            _btnContinue.Name = "_btnContinue";
            _btnContinue.Size = new Size(123, 26);
            _btnContinue.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnContinue.TabIndex = 8;
            _btnContinue.TextColor = SystemColors.GrayText;
            _btnContinue.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnContinue.Vertical_Alignment = StringAlignment.Center;
            _btnContinue.Click += _btnContinue_Click;
            // 
            // _btnGithub
            // 
            _btnGithub.BackgroundColor = Color.FromArgb(26, 26, 26);
            _btnGithub.ButtonImage = Properties.Resources.Github;
            _btnGithub.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnGithub.ButtonText = "";
            _btnGithub.ClickBackColor = Color.FromArgb(26, 26, 26);
            _btnGithub.ClickTextColor = Color.FromArgb(26, 26, 26);
            _btnGithub.CornerRadius = 5;
            _btnGithub.Horizontal_Alignment = StringAlignment.Center;
            _btnGithub.HoverBackgroundColor = Color.FromArgb(26, 26, 26);
            _btnGithub.HoverTextColor = Color.Gainsboro;
            _btnGithub.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Center;
            _btnGithub.Location = new Point(705, 422);
            _btnGithub.Name = "_btnGithub";
            _btnGithub.Size = new Size(47, 39);
            _btnGithub.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnGithub.TabIndex = 9;
            _btnGithub.TextColor = SystemColors.GrayText;
            _btnGithub.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnGithub.Vertical_Alignment = StringAlignment.Center;
            _btnGithub.Click += _btnGithub_Click;
            // 
            // _spinnerFeedback
            // 
            _spinnerFeedback.BackColor = Color.FromArgb(26, 26, 26);
            _spinnerFeedback.BackgroundImageLayout = ImageLayout.None;
            _spinnerFeedback.Location = new Point(329, 174);
            _spinnerFeedback.Maximum = 100;
            _spinnerFeedback.Minimum = 45;
            _spinnerFeedback.Name = "_spinnerFeedback";
            _spinnerFeedback.Size = new Size(100, 100);
            _spinnerFeedback.Style = ReaLTaiizor.Enum.Poison.ColorStyle.White;
            _spinnerFeedback.TabIndex = 10;
            _spinnerFeedback.Text = ".";
            _spinnerFeedback.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            _spinnerFeedback.UseCustomBackColor = true;
            _spinnerFeedback.UseSelectable = true;
            _spinnerFeedback.Value = 60;
            _spinnerFeedback.Visible = false;
            // 
            // _ucDownloadGMOD
            // 
            _ucDownloadGMOD.BackColor = Color.FromArgb(26, 26, 26);
            _ucDownloadGMOD.Enabled = false;
            _ucDownloadGMOD.Location = new Point(0, 0);
            _ucDownloadGMOD.Name = "_ucDownloadGMOD";
            _ucDownloadGMOD.Size = new Size(752, 464);
            _ucDownloadGMOD.TabIndex = 11;
            _ucDownloadGMOD.Visible = false;
            // 
            // _tmTimer
            // 
            _tmTimer.Interval = 1000;
            // 
            // SetupAccount
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            Controls.Add(_ucDownloadGMOD);
            Controls.Add(_spinnerFeedback);
            Controls.Add(_btnGithub);
            Controls.Add(_btnContinue);
            Controls.Add(_txtPassword);
            Controls.Add(_txtUsername);
            Controls.Add(_lblDisclaimer2);
            Controls.Add(_lblDisclaimer);
            Controls.Add(_lblTitle);
            Name = "SetupAccount";
            Size = new Size(752, 464);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ReaLTaiizor.Controls.CrownLabel _lblTitle;
        private ReaLTaiizor.Controls.CrownLabel _lblDisclaimer;
        private ReaLTaiizor.Controls.CrownLabel _lblDisclaimer2;
        private ReaLTaiizor.Controls.CrownTextBox _txtPassword;
        private ReaLTaiizor.Controls.CrownTextBox _txtUsername;
        private ReaLTaiizor.Controls.ParrotButton _btnContinue;
        private ReaLTaiizor.Controls.ParrotButton _btnGithub;
        private ReaLTaiizor.Controls.PoisonProgressSpinner _spinnerFeedback;
        private DownloadGMOD _ucDownloadGMOD;
        private System.Windows.Forms.Timer _tmTimer;
    }
}
