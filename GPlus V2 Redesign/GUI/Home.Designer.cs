namespace GPlus_V2_Redesign
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
            _ControlBox = new ReaLTaiizor.Controls.NightControlBox();
            navBar1 = new GPlus_V2_Redesign.GUI.Elements.NavBar();
            SuspendLayout();
            // 
            // _ControlBox
            // 
            _ControlBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _ControlBox.BackColor = Color.Transparent;
            _ControlBox.CloseHoverColor = Color.FromArgb(199, 80, 80);
            _ControlBox.CloseHoverForeColor = Color.White;
            _ControlBox.DefaultLocation = true;
            _ControlBox.DisableMaximizeColor = Color.FromArgb(105, 105, 105);
            _ControlBox.DisableMinimizeColor = Color.FromArgb(105, 105, 105);
            _ControlBox.EnableCloseColor = Color.FromArgb(160, 160, 160);
            _ControlBox.EnableMaximizeButton = false;
            _ControlBox.EnableMaximizeColor = Color.FromArgb(160, 160, 160);
            _ControlBox.EnableMinimizeButton = true;
            _ControlBox.EnableMinimizeColor = Color.FromArgb(160, 160, 160);
            _ControlBox.Location = new Point(629, 0);
            _ControlBox.MaximizeHoverColor = Color.FromArgb(15, 255, 255, 255);
            _ControlBox.MaximizeHoverForeColor = Color.White;
            _ControlBox.MinimizeHoverColor = Color.FromArgb(15, 255, 255, 255);
            _ControlBox.MinimizeHoverForeColor = Color.White;
            _ControlBox.Name = "_ControlBox";
            _ControlBox.Size = new Size(139, 31);
            _ControlBox.TabIndex = 2;
            // 
            // navBar1
            // 
            navBar1.Dock = DockStyle.Left;
            navBar1.Location = new Point(0, 0);
            navBar1.Name = "navBar1";
            navBar1.Size = new Size(150, 464);
            navBar1.TabIndex = 3;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            ClientSize = new Size(752, 464);
            Controls.Add(navBar1);
            Controls.Add(_ControlBox);
            FormBorderStyle = FormBorderStyle.None;
            MaximumSize = new Size(1920, 1032);
            Name = "Home";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            TransparencyKey = Color.Fuchsia;
            Load += Home_Load;
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.NightControlBox _ControlBox;
        private GUI.Elements.NavBar navBar1;
    }
}
