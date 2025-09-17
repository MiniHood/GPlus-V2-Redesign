namespace GPlus
{
    partial class Home
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _ctrlControlBox = new ReaLTaiizor.Controls.NightControlBox();
            _ucNavBar = new GPlus.GUI.Elements.NavBar();
            _ucClients = new GPlus.GUI.Elements.Clients();
            _ucDashboard = new GPlus.GUI.Elements.Dashboard();
            _ucServers = new GPlus.GUI.Elements.Servers();
            _ucSettings = new GPlus.GUI.Elements.Settings();
            _ucSetup = new GPlus.GUI.Elements.Setup();
            _ucShuttingDown = new GPlus.GUI.Elements.ShuttingDown();
            SuspendLayout();
            // 
            // _ctrlControlBox
            // 
            _ctrlControlBox.BackColor = Color.Transparent;
            _ctrlControlBox.CloseHoverColor = Color.FromArgb(199, 80, 80);
            _ctrlControlBox.CloseHoverForeColor = Color.White;
            _ctrlControlBox.DefaultLocation = true;
            _ctrlControlBox.DisableMaximizeColor = Color.FromArgb(105, 105, 105);
            _ctrlControlBox.DisableMinimizeColor = Color.FromArgb(105, 105, 105);
            _ctrlControlBox.Dock = DockStyle.Right;
            _ctrlControlBox.EnableCloseColor = Color.FromArgb(160, 160, 160);
            _ctrlControlBox.EnableMaximizeButton = false;
            _ctrlControlBox.EnableMaximizeColor = Color.FromArgb(160, 160, 160);
            _ctrlControlBox.EnableMinimizeButton = true;
            _ctrlControlBox.EnableMinimizeColor = Color.FromArgb(160, 160, 160);
            _ctrlControlBox.Location = new Point(613, 0);
            _ctrlControlBox.MaximizeHoverColor = Color.FromArgb(15, 255, 255, 255);
            _ctrlControlBox.MaximizeHoverForeColor = Color.White;
            _ctrlControlBox.MinimizeHoverColor = Color.FromArgb(15, 255, 255, 255);
            _ctrlControlBox.MinimizeHoverForeColor = Color.White;
            _ctrlControlBox.Name = "_ctrlControlBox";
            _ctrlControlBox.Size = new Size(139, 31);
            _ctrlControlBox.TabIndex = 2;
            // 
            // _ucNavBar
            // 
            _ucNavBar.BackColor = Color.FromArgb(26, 26, 26);
            _ucNavBar.Dock = DockStyle.Left;
            _ucNavBar.Location = new Point(0, 0);
            _ucNavBar.Name = "_ucNavBar";
            _ucNavBar.Size = new Size(150, 464);
            _ucNavBar.TabIndex = 3;
            // 
            // _ucClients
            // 
            _ucClients.BackColor = Color.FromArgb(26, 26, 26);
            _ucClients.Location = new Point(153, 37);
            _ucClients.Name = "_ucClients";
            _ucClients.Size = new Size(599, 427);
            _ucClients.TabIndex = 4;
            // 
            // _ucDashboard
            // 
            _ucDashboard.BackColor = Color.FromArgb(26, 26, 26);
            _ucDashboard.Location = new Point(153, 37);
            _ucDashboard.Name = "_ucDashboard";
            _ucDashboard.Size = new Size(599, 427);
            _ucDashboard.TabIndex = 5;
            // 
            // _ucServers
            // 
            _ucServers.BackColor = Color.FromArgb(26, 26, 26);
            _ucServers.Location = new Point(153, 37);
            _ucServers.Name = "_ucServers";
            _ucServers.Size = new Size(599, 427);
            _ucServers.TabIndex = 6;
            // 
            // _ucSettings
            // 
            _ucSettings.BackColor = Color.FromArgb(26, 26, 26);
            _ucSettings.Location = new Point(153, 37);
            _ucSettings.Name = "_ucSettings";
            _ucSettings.Size = new Size(599, 427);
            _ucSettings.TabIndex = 7;
            // 
            // _ucSetup
            // 
            _ucSetup.BackColor = Color.FromArgb(26, 26, 26);
            _ucSetup.Location = new Point(0, 37);
            _ucSetup.Name = "_ucSetup";
            _ucSetup.Size = new Size(752, 427);
            _ucSetup.TabIndex = 8;
            // 
            // _ucShuttingDown
            // 
            _ucShuttingDown.BackColor = Color.FromArgb(26, 26, 26);
            _ucShuttingDown.Location = new Point(0, 37);
            _ucShuttingDown.Name = "_ucShuttingDown";
            _ucShuttingDown.Size = new Size(752, 427);
            _ucShuttingDown.TabIndex = 9;
            _ucShuttingDown.Visible = false;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            ClientSize = new Size(752, 464);
            Controls.Add(_ucShuttingDown);
            Controls.Add(_ucSetup);
            Controls.Add(_ucClients);
            Controls.Add(_ucNavBar);
            Controls.Add(_ctrlControlBox);
            Controls.Add(_ucSettings);
            Controls.Add(_ucServers);
            Controls.Add(_ucDashboard);
            FormBorderStyle = FormBorderStyle.None;
            MaximumSize = new Size(1920, 1032);
            Name = "Home";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GPlusV2";
            TransparencyKey = Color.Fuchsia;
            FormClosing += Home_FormClosing;
            Load += Home_Load;
            MouseDown += Home_MouseDown;
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.NightControlBox _ctrlControlBox;
        private GUI.Elements.NavBar _ucNavBar;
        private GUI.Elements.Clients _ucClients;
        private GUI.Elements.Dashboard _ucDashboard;
        private GUI.Elements.Servers _ucServers;
        private GUI.Elements.Settings _ucSettings;
        private GUI.Elements.Setup _ucSetup;
        private GUI.Elements.ShuttingDown _ucShuttingDown;
    }
}
