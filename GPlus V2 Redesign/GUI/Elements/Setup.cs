using GPlus.GUI.Helpers;
using GPlus.Source;
using GPlus.Source.Steam;
using System.Diagnostics;
using System.Runtime;

namespace GPlus.GUI.Elements
{
    public partial class Setup : UserControl
    {
        public Setup()
        {
            InitializeComponent();
        }

        void ChangeProgress(int progress)
        {
            if (_progProgressBar.InvokeRequired)
            {
                _progProgressBar.Invoke(new Action(() => _progProgressBar.Value = progress));
            }
            else
            {
                _progProgressBar.Value = progress;
            }
        }

        void ChangeSteamCMDProgress(int progress)
        {
            if (_spinnerFeedback.Visible)
            {
                _spinnerFeedback.Invoke(new Action(() => _spinnerFeedback.Visible = false));
                _progProgressBar.Invoke(new Action(() => _progProgressBar.Visible = true));
            }

            ChangeProgress(progress);
        }

        void ChangeLabelText(string text)
        {
            if (_txtFeedback.InvokeRequired)
            {
                _txtFeedback.Invoke(new Action(() =>
                {
                    _txtFeedback.Text = text;
                }));
            }
            else
            {
                _txtFeedback.Text = text;
            }
        }

        async Task<bool> StartSetup()
        {
            #region Steam Setup
            if (!SteamSetup.IsSteamInstalled())
            {
                ChangeLabelText("Downloading SteamCMD...");
                if (!await SteamSetup.DownloadSteamClient())
                {
                    ChangeLabelText("Download Failed. Please try reload the application.");
                    return false;
                }

                ChangeLabelText("Extracting SteamCMD...");
                if (!await SteamSetup.UnzipSteamClient())
                {
                    ChangeLabelText("Extraction Failed. Please try reload the application.");
                    return false;
                }

            }


            _spinnerFeedback.Invoke(new Action(() => _spinnerFeedback.Visible = true));
            _progProgressBar.Invoke(new Action(() => _progProgressBar.Visible = false));
            ChangeLabelText("Awaiting SteamCMD Update...");
            Debug.WriteLine("Awaiting SteamCMD Update...");
            await SteamSetup.AllowSteamUpdate();
            #endregion

            _progProgressBar.Invoke(new Action(() => _progProgressBar.Visible = false));
            _spinnerFeedback.Invoke(new Action(() => _spinnerFeedback.Visible = false));
            ChangeLabelText("Initialization Complete!");
            await Task.Delay(1000);

            this.Dispose();

            return true;
        }

        private void Setup_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            if (SteamSetup.IsSteamInstalled())
            { this.Dispose(); return; }

            SteamSetup.OnDownloadProgressChanged += (object? s, int e) => { ChangeProgress(e); };
            SteamSetup.OnZipProgressChanged += (object? s, int e) => { ChangeProgress(e); };
            SteamSetup.SteamCMDUpdateProgressChange += (object? s, int e) => { ChangeSteamCMDProgress(e); };

            Task.Run(async () => { await StartSetup(); });
        }
    }
}
