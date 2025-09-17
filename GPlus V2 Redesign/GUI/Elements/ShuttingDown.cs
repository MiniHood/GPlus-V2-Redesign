using GPlus.Game.Clients;
using GPlus.GUI.Helpers;
using GPlus.Source.Sandboxing;
using GPlus.Source.Steam;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPlus.GUI.Elements
{
    public partial class ShuttingDown : UserControl
    {
        EventHandler<string>? OnShutdownUpdate;
        public static ShuttingDown Instance;

        public ShuttingDown()
        {
            InitializeComponent();
            Instance = this;
        }

        void OnShutdownUpdateEvent(object sender, string message)
        {
            _lblFeedback.Text = message;
        }
        static int _timerTickCount = 0;

        private void SetupTimer()
        {
            _timerTickCount = 0;
            _Timer.Enabled = true;

            _Timer.Tick -= TimerOnTick; // avoid duplicate event handlers
            _Timer.Tick += TimerOnTick;
        }

        private void TimerOnTick(object? sender, EventArgs e)
        {
            _timerTickCount++;
            if (_timerTickCount == 5)
            {
                _lblTitle.Text = "Forcing SteamCMD shutdown.";
                _Timer.Stop();
                _timerTickCount = 0;

                SteamCMD.ForceStopSteamCMD();
            }
        }

        public async static void OnProcessExit(object sender, EventArgs e)
        {
            UserControlLoader.ShuttingDown.Visible = true;
            UserControlLoader.ShuttingDown.BringToFront();

            if (SteamCMD.CurrentSteamCMDInstance != null)
            {
                Instance._lblTitle.Text = "Safely shutting SteamCMD down.";
                // Attempt to stop recovery loops, but if it doesn't end in time, force close it and clear logs
                // to avoid recovery loops on next startup.
                await SteamCMD.CurrentSteamCMDInstance.StandardInput.WriteLineAsync("quit");
                Instance._Timer.Start();
            }

            Instance._lblTitle.Text = "Shutting down clients.";
            await ClientManager.OnShutdown();
            Instance._lblTitle.Text = "Shutting down Sandboxies.";
            await SandboxieManager.OnShutdown();
        }

        private void ShuttingDown_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            OnShutdownUpdate += OnShutdownUpdateEvent;
        }
    }
}
