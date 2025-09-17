namespace GPlus.GUI.Elements
{
    partial class ShuttingDown
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
            _spinnerFeedback = new ReaLTaiizor.Controls.PoisonProgressSpinner();
            _lblFeedback = new ReaLTaiizor.Controls.CrownLabel();
            _Timer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = DockStyle.Top;
            _lblTitle.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lblTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblTitle.Location = new Point(0, 0);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(752, 37);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Shutting Down";
            _lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _spinnerFeedback
            // 
            _spinnerFeedback.BackColor = Color.FromArgb(26, 26, 26);
            _spinnerFeedback.BackgroundImageLayout = ImageLayout.None;
            _spinnerFeedback.Location = new Point(332, 157);
            _spinnerFeedback.Maximum = 100;
            _spinnerFeedback.Minimum = 45;
            _spinnerFeedback.Name = "_spinnerFeedback";
            _spinnerFeedback.Size = new Size(100, 100);
            _spinnerFeedback.Style = ReaLTaiizor.Enum.Poison.ColorStyle.White;
            _spinnerFeedback.TabIndex = 4;
            _spinnerFeedback.Text = ".";
            _spinnerFeedback.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            _spinnerFeedback.UseCustomBackColor = true;
            _spinnerFeedback.UseSelectable = true;
            _spinnerFeedback.Value = 60;
            _spinnerFeedback.Visible = false;
            // 
            // _lblFeedback
            // 
            _lblFeedback.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lblFeedback.ForeColor = Color.FromArgb(220, 220, 220);
            _lblFeedback.Location = new Point(0, 273);
            _lblFeedback.Name = "_lblFeedback";
            _lblFeedback.Size = new Size(752, 37);
            _lblFeedback.TabIndex = 5;
            _lblFeedback.Text = "Shutting down Sandboxies.";
            _lblFeedback.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _Timer
            // 
            _Timer.Interval = 1000;
            // 
            // ShuttingDown
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            Controls.Add(_lblFeedback);
            Controls.Add(_spinnerFeedback);
            Controls.Add(_lblTitle);
            Name = "ShuttingDown";
            Size = new Size(752, 464);
            Load += ShuttingDown_Load;
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.CrownLabel _lblTitle;
        private ReaLTaiizor.Controls.PoisonProgressSpinner _spinnerFeedback;
        private ReaLTaiizor.Controls.CrownLabel _lblFeedback;
        public System.Windows.Forms.Timer _Timer;
    }
}
