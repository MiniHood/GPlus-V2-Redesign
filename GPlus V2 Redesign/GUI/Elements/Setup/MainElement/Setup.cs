using GPlus.Game.Clients;
using GPlus.Source.General;
using GPlus.Source.Interprocess;
using GPlus.Source.Steam;
using System.Diagnostics;
using WebSocketSharp.Server;

namespace GPlus.GUI.Elements
{
    public partial class Setup : UserControl
    {
        public static WebSocketServer server;
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
            #region Settings
            ChangeLabelText("Loading Settings...");
            await SettingsManager.LoadSettingsAsync();
            #endregion

            #region User Controls
            ChangeLabelText("Loading User Controls...");
            Home.LoadUserControls();
            #endregion

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

            // wait for steamcmd to close to avoid recovery loop and file locks
            ChangeLabelText("Waiting for SteamCMD Shutdown...");
            Debug.WriteLine("Waiting for SteamCMD Shutdown...");
            while (SteamCMD.IsSteamCMDRunning())
            {
                await Task.Delay(1000);
            }
            #endregion

            #region GMOD Setup
            _progProgressBar.Invoke(new Action(() => _progProgressBar.Visible = false));
            _spinnerFeedback.Invoke(new Action(() => _spinnerFeedback.Visible = false));

            if (!SteamSetup.IsGMODInstalled())
            {
                _ucSetupAccount.Invoke(new Action(() => _ucSetupAccount.Visible = true));
                _ucSetupAccount.Invoke(new Action(() => _ucSetupAccount.BringToFront()));
                return true; // hand off to set up account uc
            }
            #endregion

            #region Communication Setup
            ChangeLabelText("Initializing WebSocket Server");
            _ = CommunicationTCP.StartAsync();
            Debug.WriteLine("Started websocket server");
            #endregion

            #region Load Clients
            ChangeLabelText("Loading saved clients...");
            await ClientManager.LoadSavedClients();
            #endregion

            ChangeLabelText("Initialization Complete!");
            await Task.Delay(1000);

            Dispose();

            return true;
        }

        private void Setup_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            //if (SteamSetup.IsSteamInstalled())
            //{ this.Dispose(); return; }

            SteamSetup.OnDownloadProgressChanged += (object? s, int e) => { ChangeProgress(e); };
            SteamSetup.OnZipProgressChanged += (object? s, int e) => { ChangeProgress(e); };
            SteamSetup.OnSteamCMDUpdateProgressChanged += (object? s, int e) => { ChangeSteamCMDProgress(e); };

            Task.Run(async () => { await StartSetup(); });
        }
    }
}
