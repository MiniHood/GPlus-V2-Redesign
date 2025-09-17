using GPlus.Game.Clients;
using GPlus.GUI.Helpers;
using GPlus.Source.Sandboxing;
using GPlus.Source.Steam;
using System;
using System.Diagnostics;

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

        public static bool ShutdownComplete = false;
        public static int TickCount = 0;
        public async static Task OnProcessExit()
        {
            if (ShutdownComplete)
                return;

            UserControlLoader.ShuttingDown.Visible = true;
            UserControlLoader.ShuttingDown.BringToFront();

            if (SteamCMD.CurrentSteamCMDInstance != null)
            {
                Instance._lblFeedback.Text = "Shutting SteamCMD down.";
                // Attempt to stop recovery loops, but if it doesn't end in time, force close it and clear logs
                // to avoid recovery loops on next startup.

                await SteamCMD.CurrentSteamCMDInstance.StandardInput.WriteLineAsync("quit");


                while (SteamCMD.CurrentSteamCMDInstance != null)
                {
                    TickCount++;

                    if (TickCount > 10)
                    {
                        Instance._lblFeedback.Text = "Forcing SteamCMD shutdown.";
                        SteamCMD.ForceStopSteamCMD();
                        break;
                    }

                    await Task.Delay(1000);
                }
            }

            Instance._lblFeedback.Text = "Shutting down Sandboxies.";
            await SandboxieManager.OnShutdown();
            Instance._lblFeedback.Text = "Shutting down clients.";
            await ClientManager.OnShutdown();

            Instance._lblFeedback.Text = "Shutdown complete.";
            await Task.Delay(1000);

            ShutdownComplete = true;
            Home.FormShutdownAllowed = true;
            Application.Exit();
        }

        private void ShuttingDown_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            OnShutdownUpdate += OnShutdownUpdateEvent;
        }
    }
}
