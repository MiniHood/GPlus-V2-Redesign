using GPlus.Game.Clients;
using System.Diagnostics;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace GPlus.Source.Steam
{
    internal class SteamCMD
    {
        public static string GetSteamCMDPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "steamcmd\\steamcmd.exe";
        }

        private static Process StartSteamCMD(string Args)
        {
            if (!SteamSetup.IsSteamInstalled())
                throw new Exception("SteamCMD is not installed.");

            var psi = new ProcessStartInfo
            {
                FileName = GetSteamCMDPath(),
                Arguments = Args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            if (process == null)
                throw new Exception("Failed to start SteamCMD process.");

            return process;
        }

        public static async Task<bool> DoesClientHave2FA(Client client)
        {
            bool Has2FA = false;
            Process process = StartSteamCMD($"+login {client.LoginDetails.Username} {client.LoginDetails.Password} +quit");
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data) || e == null)
                    return;

                if (e.Data.Contains("This account is protected by") || e.Data.Contains("Please confirm the login"))
                    Has2FA = true;
            };

            if (Has2FA)
                return true;
            return false;
        }
    }
}
