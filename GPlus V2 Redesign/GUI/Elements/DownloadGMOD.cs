using GPlus.Source.Enums;
using GPlus.Source.Steam;
using GPlus.Source.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPlus.GUI.Elements
{
    public partial class DownloadGMOD : UserControl
    {
        public static DownloadGMOD Instance = null;

        public DownloadGMOD()
        {
            InitializeComponent();
            Instance = this;
        }

        private void _lblDownloading_Click(object sender, EventArgs e)
        {

        }

        public static void DownloadProgressChange(object sender, GeneralSteamResponse response)
        {
            if(response.response == ClientResponse.SUCCESSFUL)
            {
                // Download is complete
            }

            if (response.Progress == null)
                return;

            if (response.responseType == ResponseType.Verifying)
                Instance._lblFeedback.Invoke(new Action(() => Instance._lblFeedback.Text = "Verifying game files."));
            else if (response.responseType == ResponseType.Downloading)
                Instance._lblFeedback.Invoke(new Action(() => Instance._lblFeedback.Text = "Downloading game files."));
            else if (response.responseType == ResponseType.Commiting)
                Instance._lblFeedback.Invoke(new Action(() => Instance._lblFeedback.Text = "Commiting game files."));

            Instance._spinnerFeedback.Invoke(new Action(() => Instance._spinnerFeedback.Visible = false));

            Instance._progProgressBar.Invoke(new Action(() => Instance._progProgressBar.Value = (int)response.Progress));
        }

        private void DownloadGMOD_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            SteamCMD.OnSteamCMDResponseUpdated += DownloadProgressChange;
        }

        private void _progProgressBar_Click(object sender, EventArgs e)
        {

        }
    }
}
