namespace GPlus.GUI.Elements
{
    partial class NavBar
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
            _NavBar = new ReaLTaiizor.Controls.NightPanel();
            _btnSettings = new ReaLTaiizor.Controls.Button();
            _btnClients = new ReaLTaiizor.Controls.Button();
            _btnServers = new ReaLTaiizor.Controls.Button();
            _btnDashboard = new ReaLTaiizor.Controls.Button();
            _NavBar.SuspendLayout();
            SuspendLayout();
            // 
            // _NavBar
            // 
            _NavBar.Controls.Add(_btnSettings);
            _NavBar.Controls.Add(_btnClients);
            _NavBar.Controls.Add(_btnServers);
            _NavBar.Controls.Add(_btnDashboard);
            _NavBar.Dock = DockStyle.Left;
            _NavBar.ForeColor = Color.FromArgb(250, 250, 250);
            _NavBar.LeftSideColor = Color.FromArgb(26, 26, 26);
            _NavBar.Location = new Point(0, 0);
            _NavBar.Name = "_NavBar";
            _NavBar.RightSideColor = Color.FromArgb(26, 26, 26);
            _NavBar.Side = ReaLTaiizor.Controls.NightPanel.PanelSide.Right;
            _NavBar.Size = new Size(157, 464);
            _NavBar.TabIndex = 4;
            // 
            // _btnSettings
            // 
            _btnSettings.BackColor = Color.Transparent;
            _btnSettings.BorderColor = Color.FromArgb(26, 26, 26);
            _btnSettings.Dock = DockStyle.Top;
            _btnSettings.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            _btnSettings.EnteredColor = Color.FromArgb(32, 34, 37);
            _btnSettings.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _btnSettings.Image = Properties.Resources.setting;
            _btnSettings.ImageAlign = ContentAlignment.MiddleCenter;
            _btnSettings.InactiveColor = Color.FromArgb(26, 26, 26);
            _btnSettings.Location = new Point(0, 348);
            _btnSettings.Name = "_btnSettings";
            _btnSettings.PressedBorderColor = Color.FromArgb(45, 45, 45);
            _btnSettings.PressedColor = Color.FromArgb(36, 36, 36);
            _btnSettings.Size = new Size(157, 116);
            _btnSettings.TabIndex = 8;
            _btnSettings.TextAlignment = StringAlignment.Center;
            _btnSettings.Click += _btnSettings_Click;
            // 
            // _btnClients
            // 
            _btnClients.BackColor = Color.Transparent;
            _btnClients.BorderColor = Color.FromArgb(26, 26, 26);
            _btnClients.Dock = DockStyle.Top;
            _btnClients.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            _btnClients.EnteredColor = Color.FromArgb(32, 34, 37);
            _btnClients.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _btnClients.Image = Properties.Resources.Users;
            _btnClients.ImageAlign = ContentAlignment.MiddleCenter;
            _btnClients.InactiveColor = Color.FromArgb(26, 26, 26);
            _btnClients.Location = new Point(0, 232);
            _btnClients.Name = "_btnClients";
            _btnClients.PressedBorderColor = Color.FromArgb(45, 45, 45);
            _btnClients.PressedColor = Color.FromArgb(36, 36, 36);
            _btnClients.Size = new Size(157, 116);
            _btnClients.TabIndex = 7;
            _btnClients.TextAlignment = StringAlignment.Center;
            _btnClients.Click += _btnClients_Click;
            // 
            // _btnServers
            // 
            _btnServers.BackColor = Color.Transparent;
            _btnServers.BorderColor = Color.FromArgb(26, 26, 26);
            _btnServers.Dock = DockStyle.Top;
            _btnServers.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            _btnServers.EnteredColor = Color.FromArgb(32, 34, 37);
            _btnServers.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _btnServers.Image = Properties.Resources.Servers;
            _btnServers.ImageAlign = ContentAlignment.MiddleCenter;
            _btnServers.InactiveColor = Color.FromArgb(26, 26, 26);
            _btnServers.Location = new Point(0, 116);
            _btnServers.Name = "_btnServers";
            _btnServers.PressedBorderColor = Color.FromArgb(45, 45, 45);
            _btnServers.PressedColor = Color.FromArgb(36, 36, 36);
            _btnServers.Size = new Size(157, 116);
            _btnServers.TabIndex = 6;
            _btnServers.TextAlignment = StringAlignment.Center;
            _btnServers.Click += _btnServers_Click;
            // 
            // _btnDashboard
            // 
            _btnDashboard.BackColor = Color.Transparent;
            _btnDashboard.BorderColor = Color.FromArgb(26, 26, 26);
            _btnDashboard.Dock = DockStyle.Top;
            _btnDashboard.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            _btnDashboard.EnteredColor = Color.FromArgb(32, 34, 37);
            _btnDashboard.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _btnDashboard.Image = Properties.Resources.Dashboard;
            _btnDashboard.ImageAlign = ContentAlignment.MiddleCenter;
            _btnDashboard.InactiveColor = Color.FromArgb(26, 26, 26);
            _btnDashboard.Location = new Point(0, 0);
            _btnDashboard.Name = "_btnDashboard";
            _btnDashboard.PressedBorderColor = Color.FromArgb(45, 45, 45);
            _btnDashboard.PressedColor = Color.FromArgb(36, 36, 36);
            _btnDashboard.Size = new Size(157, 116);
            _btnDashboard.TabIndex = 5;
            _btnDashboard.TextAlignment = StringAlignment.Center;
            _btnDashboard.Click += _btnDashboard_Click;
            // 
            // NavBar
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            Controls.Add(_NavBar);
            Name = "NavBar";
            Size = new Size(150, 464);
            _NavBar.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.NightPanel _NavBar;
        private ReaLTaiizor.Controls.Button _btnSettings;
        private ReaLTaiizor.Controls.Button _btnClients;
        private ReaLTaiizor.Controls.Button _btnServers;
        private ReaLTaiizor.Controls.Button _btnDashboard;
    }
}
