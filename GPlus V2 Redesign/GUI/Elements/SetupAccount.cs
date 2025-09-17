using GPlus.Source.Enums;
using GPlus.Source.Steam;
using GPlus.Source.Structs;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPlus.GUI.Elements
{
    public partial class SetupAccount : UserControl
    {
        private int _timerTickCount;
        public static SetupAccount Instance;
        public SetupAccount()
        {
            InitializeComponent();
            Instance = this;
        }

        private void HideControls()
        {
            _btnContinue.Visible = false;
            _txtUsername.Visible = false;
            _txtPassword.Visible = false;
            _lblTitle.Text = "Checking 2FA Status...";
            _spinnerFeedback.Visible = true;
            _lblDisclaimer.Visible = false;
            _lblDisclaimer2.Visible = false;
        }

        private void ShowControls(string message)
        {
            _btnContinue.Visible = true;
            _txtUsername.Visible = true;
            _txtPassword.Visible = true;
            _lblTitle.Text = message;
            _spinnerFeedback.Visible = false;
            _lblDisclaimer.Visible = true;
            _lblDisclaimer2.Visible = true;
        }

        private void SetupTimer()
        {
            _timerTickCount = 0;
            _tmTimer.Enabled = true;

            _tmTimer.Tick -= TimerOnTick; // avoid duplicate event handlers
            _tmTimer.Tick += TimerOnTick;
        }

        private void TimerOnTick(object? sender, EventArgs e)
        {
            _timerTickCount++;
            if (_timerTickCount == 10)
            {
                _lblTitle.Text = "Apologies for the long wait...";
                _tmTimer.Stop();
                _timerTickCount = 0;
            }
        }

        private async void _btnContinue_Click(object sender, EventArgs e)
        {
            HideControls();
            SetupTimer();

            var login = new LoginDetails
            {
                Username = _txtUsername.Text.Trim(),
                Password = _txtPassword.Text.Trim()
            };

            GeneralSteamResponse result;
            try
            {
                result = await SteamCMD.DoesClientHave2FA(login);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] SteamCMD check failed: {ex.Message}");
                ShowControls("Unable to check 2FA. Please try again later.");
                return;
            }

            HandleSteamResponse(result);
        }

        private async void HandleSteamResponse(GeneralSteamResponse result)
        {
            switch (result.response)
            {
                case ClientResponse.AUTHENABLED:
                    Debug.WriteLine("Account has 2FA enabled.");
                    ShowControls("2FA is enabled on this account.");
                    _txtUsername.Clear();
                    _txtPassword.Clear();
                    break;

                case ClientResponse.INVALIDPASSWORD:
                    Debug.WriteLine("Invalid password.");
                    ShowControls("Invalid password. Please try again.");
                    _txtPassword.Clear();
                    break;

                case ClientResponse.UNKNOWNERROR:
                    Debug.WriteLine("Unknown error.");
                    ShowControls("Something went wrong. Try again later.");
                    break;

                case ClientResponse.SUCCESSFUL:
                    Debug.WriteLine("2FA not enabled, continuing.");
                    _ucDownloadGMOD.Enabled = true;
                    _ucDownloadGMOD.Visible = true;
                    _ucDownloadGMOD.BringToFront();
                    await SteamCMD.DownloadGarrysMod(new LoginDetails { Username = _txtUsername.Text, Password = _txtPassword.Text });
                    break;

                default:
                    Debug.WriteLine($"Unhandled response: {result.response}");
                    ShowControls("Unhandled response: " + result.response);
                    break;
            }

            _tmTimer.Stop();
            _tmTimer.Enabled = false;
            _timerTickCount = 0;
        }

        private void _btnGithub_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/MiniHood/GPlus-V2-Redesign",
                UseShellExecute = true
            };

            Process.Start(psi);
        }
    }
}
