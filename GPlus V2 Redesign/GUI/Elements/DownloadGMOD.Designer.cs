namespace GPlus.GUI.Elements
{
    partial class DownloadGMOD
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
            _lblDownloading = new ReaLTaiizor.Controls.CrownLabel();
            _lblDisclaimer = new ReaLTaiizor.Controls.CrownLabel();
            _progProgressBar = new ReaLTaiizor.Controls.ForeverProgressBar();
            _lblFeedback = new ReaLTaiizor.Controls.CrownLabel();
            _spinnerFeedback = new ReaLTaiizor.Controls.PoisonProgressSpinner();
            SuspendLayout();
            // 
            // _lblDownloading
            // 
            _lblDownloading.BackColor = Color.FromArgb(26, 26, 26);
            _lblDownloading.Dock = DockStyle.Top;
            _lblDownloading.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lblDownloading.ForeColor = Color.FromArgb(220, 220, 220);
            _lblDownloading.Location = new Point(0, 0);
            _lblDownloading.Name = "_lblDownloading";
            _lblDownloading.Size = new Size(752, 39);
            _lblDownloading.TabIndex = 3;
            _lblDownloading.Text = "Downloading Garry's Mod";
            _lblDownloading.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblDisclaimer
            // 
            _lblDisclaimer.BackColor = Color.FromArgb(26, 26, 26);
            _lblDisclaimer.Dock = DockStyle.Fill;
            _lblDisclaimer.ForeColor = Color.FromArgb(220, 220, 220);
            _lblDisclaimer.Location = new Point(0, 0);
            _lblDisclaimer.Name = "_lblDisclaimer";
            _lblDisclaimer.Size = new Size(752, 464);
            _lblDisclaimer.TabIndex = 4;
            _lblDisclaimer.Text = "Please wait. This is a one time setup.";
            _lblDisclaimer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _progProgressBar
            // 
            _progProgressBar.BackColor = Color.FromArgb(26, 26, 26);
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
            _progProgressBar.TabIndex = 5;
            _progProgressBar.Text = "Downloading";
            _progProgressBar.Value = 0;
            _progProgressBar.Click += _progProgressBar_Click;
            // 
            // _lblFeedback
            // 
            _lblFeedback.BackColor = Color.FromArgb(26, 26, 26);
            _lblFeedback.ForeColor = Color.FromArgb(220, 220, 220);
            _lblFeedback.Location = new Point(0, 248);
            _lblFeedback.Name = "_lblFeedback";
            _lblFeedback.Size = new Size(752, 27);
            _lblFeedback.TabIndex = 6;
            _lblFeedback.Text = "Starting download.";
            _lblFeedback.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _spinnerFeedback
            // 
            _spinnerFeedback.BackColor = Color.FromArgb(26, 26, 26);
            _spinnerFeedback.BackgroundImageLayout = ImageLayout.None;
            _spinnerFeedback.Location = new Point(324, 278);
            _spinnerFeedback.Maximum = 100;
            _spinnerFeedback.Minimum = 45;
            _spinnerFeedback.Name = "_spinnerFeedback";
            _spinnerFeedback.Size = new Size(100, 100);
            _spinnerFeedback.Style = ReaLTaiizor.Enum.Poison.ColorStyle.White;
            _spinnerFeedback.TabIndex = 7;
            _spinnerFeedback.Text = ".";
            _spinnerFeedback.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            _spinnerFeedback.UseCustomBackColor = true;
            _spinnerFeedback.UseSelectable = true;
            _spinnerFeedback.Value = 60;
            // 
            // DownloadGMOD
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            Controls.Add(_spinnerFeedback);
            Controls.Add(_lblFeedback);
            Controls.Add(_progProgressBar);
            Controls.Add(_lblDownloading);
            Controls.Add(_lblDisclaimer);
            Name = "DownloadGMOD";
            Size = new Size(752, 464);
            Load += DownloadGMOD_Load;
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.CrownLabel _lblDownloading;
        private ReaLTaiizor.Controls.CrownLabel _lblDisclaimer;
        private ReaLTaiizor.Controls.ForeverProgressBar _progProgressBar;
        private ReaLTaiizor.Controls.CrownLabel _lblFeedback;
        private ReaLTaiizor.Controls.PoisonProgressSpinner _spinnerFeedback;
    }
}
