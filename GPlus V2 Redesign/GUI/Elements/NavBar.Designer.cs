namespace GPlus_V2_Redesign.GUI.Elements
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
            button4 = new ReaLTaiizor.Controls.Button();
            button3 = new ReaLTaiizor.Controls.Button();
            button2 = new ReaLTaiizor.Controls.Button();
            button1 = new ReaLTaiizor.Controls.Button();
            _NavBar.SuspendLayout();
            SuspendLayout();
            // 
            // _NavBar
            // 
            _NavBar.Controls.Add(button1);
            _NavBar.Controls.Add(button4);
            _NavBar.Controls.Add(button3);
            _NavBar.Controls.Add(button2);
            _NavBar.Dock = DockStyle.Left;
            _NavBar.ForeColor = Color.FromArgb(250, 250, 250);
            _NavBar.LeftSideColor = Color.FromArgb(32, 32, 32);
            _NavBar.Location = new Point(0, 0);
            _NavBar.Name = "_NavBar";
            _NavBar.RightSideColor = Color.FromArgb(32, 32, 32);
            _NavBar.Side = ReaLTaiizor.Controls.NightPanel.PanelSide.Left;
            _NavBar.Size = new Size(157, 464);
            _NavBar.TabIndex = 4;
            // 
            // button4
            // 
            button4.BackColor = Color.Transparent;
            button4.BorderColor = Color.FromArgb(32, 34, 37);
            button4.Dock = DockStyle.Top;
            button4.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            button4.EnteredColor = Color.FromArgb(32, 34, 37);
            button4.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button4.Image = null;
            button4.ImageAlign = ContentAlignment.MiddleLeft;
            button4.InactiveColor = Color.FromArgb(32, 34, 37);
            button4.Location = new Point(0, 232);
            button4.Name = "button4";
            button4.PressedBorderColor = Color.FromArgb(45, 45, 45);
            button4.PressedColor = Color.FromArgb(36, 36, 36);
            button4.Size = new Size(157, 116);
            button4.TabIndex = 7;
            button4.Text = "Clients";
            button4.TextAlignment = StringAlignment.Center;
            // 
            // button3
            // 
            button3.BackColor = Color.Transparent;
            button3.BorderColor = Color.FromArgb(32, 34, 37);
            button3.Dock = DockStyle.Top;
            button3.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            button3.EnteredColor = Color.FromArgb(32, 34, 37);
            button3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.Image = null;
            button3.ImageAlign = ContentAlignment.MiddleLeft;
            button3.InactiveColor = Color.FromArgb(32, 34, 37);
            button3.Location = new Point(0, 116);
            button3.Name = "button3";
            button3.PressedBorderColor = Color.FromArgb(45, 45, 45);
            button3.PressedColor = Color.FromArgb(36, 36, 36);
            button3.Size = new Size(157, 116);
            button3.TabIndex = 6;
            button3.Text = "Servers";
            button3.TextAlignment = StringAlignment.Center;
            // 
            // button2
            // 
            button2.BackColor = Color.Transparent;
            button2.BorderColor = Color.FromArgb(32, 34, 37);
            button2.Dock = DockStyle.Top;
            button2.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            button2.EnteredColor = Color.FromArgb(32, 34, 37);
            button2.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Image = null;
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.InactiveColor = Color.FromArgb(32, 34, 37);
            button2.Location = new Point(0, 0);
            button2.Name = "button2";
            button2.PressedBorderColor = Color.FromArgb(45, 45, 45);
            button2.PressedColor = Color.FromArgb(36, 36, 36);
            button2.Size = new Size(157, 116);
            button2.TabIndex = 5;
            button2.Text = "Dashboard";
            button2.TextAlignment = StringAlignment.Center;
            // 
            // button1
            // 
            button1.BackColor = Color.Transparent;
            button1.BorderColor = Color.FromArgb(32, 34, 37);
            button1.Dock = DockStyle.Top;
            button1.EnteredBorderColor = Color.FromArgb(42, 42, 42);
            button1.EnteredColor = Color.FromArgb(32, 34, 37);
            button1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Image = null;
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.InactiveColor = Color.FromArgb(32, 34, 37);
            button1.Location = new Point(0, 348);
            button1.Name = "button1";
            button1.PressedBorderColor = Color.FromArgb(45, 45, 45);
            button1.PressedColor = Color.FromArgb(36, 36, 36);
            button1.Size = new Size(157, 116);
            button1.TabIndex = 8;
            button1.Text = "Settings";
            button1.TextAlignment = StringAlignment.Center;
            // 
            // NavBar
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_NavBar);
            Name = "NavBar";
            Size = new Size(150, 464);
            _NavBar.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.NightPanel _NavBar;
        private ReaLTaiizor.Controls.Button button1;
        private ReaLTaiizor.Controls.Button button4;
        private ReaLTaiizor.Controls.Button button3;
        private ReaLTaiizor.Controls.Button button2;
    }
}
