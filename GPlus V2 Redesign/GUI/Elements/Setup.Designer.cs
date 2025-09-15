namespace GPlus.GUI.Elements
{
    partial class Setup
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
            _progProgressBar = new ReaLTaiizor.Controls.ForeverProgressBar();
            _txtOneTime = new ReaLTaiizor.Controls.CrownLabel();
            _txtFeedback = new ReaLTaiizor.Controls.CrownLabel();
            _spinnerFeedback = new ReaLTaiizor.Controls.PoisonProgressSpinner();
            SuspendLayout();
            // 
            // _progProgressBar
            // 
            _progProgressBar.BackColor = Color.Transparent;
            _progProgressBar.BaseColor = Color.FromArgb(45, 47, 49);
            _progProgressBar.DarkerProgress = Color.FromArgb(23, 148, 92);
            _progProgressBar.Dock = DockStyle.Bottom;
            _progProgressBar.ForeColor = Color.FromArgb(35, 168, 109);
            _progProgressBar.Location = new Point(0, 422);
            _progProgressBar.Maximum = 100;
            _progProgressBar.MoveBalloon = true;
            _progProgressBar.Name = "_progProgressBar";
            _progProgressBar.Pattern = true;
            _progProgressBar.PercentSign = true;
            _progProgressBar.ProgressColor = Color.FromArgb(35, 168, 109);
            _progProgressBar.ShowBalloon = true;
            _progProgressBar.Size = new Size(752, 42);
            _progProgressBar.TabIndex = 0;
            _progProgressBar.Text = "Downloading";
            _progProgressBar.Value = 0;
            // 
            // _txtOneTime
            // 
            _txtOneTime.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _txtOneTime.ForeColor = Color.FromArgb(220, 220, 220);
            _txtOneTime.Location = new Point(0, 161);
            _txtOneTime.Name = "_txtOneTime";
            _txtOneTime.Size = new Size(752, 58);
            _txtOneTime.TabIndex = 1;
            _txtOneTime.Text = "Initializing.";
            _txtOneTime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _txtFeedback
            // 
            _txtFeedback.Dock = DockStyle.Fill;
            _txtFeedback.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _txtFeedback.ForeColor = Color.FromArgb(220, 220, 220);
            _txtFeedback.Location = new Point(0, 0);
            _txtFeedback.Name = "_txtFeedback";
            _txtFeedback.Size = new Size(752, 464);
            _txtFeedback.TabIndex = 2;
            _txtFeedback.Text = "Downloading...";
            _txtFeedback.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _spinnerFeedback
            // 
            _spinnerFeedback.BackColor = Color.FromArgb(26, 26, 26);
            _spinnerFeedback.BackgroundImageLayout = ImageLayout.None;
            _spinnerFeedback.Location = new Point(324, 286);
            _spinnerFeedback.Maximum = 100;
            _spinnerFeedback.Minimum = 45;
            _spinnerFeedback.Name = "_spinnerFeedback";
            _spinnerFeedback.Size = new Size(100, 100);
            _spinnerFeedback.Style = ReaLTaiizor.Enum.Poison.ColorStyle.White;
            _spinnerFeedback.TabIndex = 3;
            _spinnerFeedback.Text = ".";
            _spinnerFeedback.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            _spinnerFeedback.UseCustomBackColor = true;
            _spinnerFeedback.UseSelectable = true;
            _spinnerFeedback.Value = 60;
            _spinnerFeedback.Visible = false;
            // 
            // Setup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            Controls.Add(_spinnerFeedback);
            Controls.Add(_txtOneTime);
            Controls.Add(_progProgressBar);
            Controls.Add(_txtFeedback);
            Name = "Setup";
            Size = new Size(752, 464);
            Load += Setup_Load;
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.ForeverProgressBar _progProgressBar;
        private ReaLTaiizor.Controls.CrownLabel _txtOneTime;
        private ReaLTaiizor.Controls.CrownLabel _txtFeedback;
        private ReaLTaiizor.Controls.PoisonProgressSpinner _spinnerFeedback;
    }
}
