using GPlus.Source.Enums;
using GPlus.Source.Sandboxing;
using GPlus.Source.Structs;
using System.Diagnostics;

namespace GPlus.Source.Steam
{
    internal class SteamCMD
    {
        public static event EventHandler<int>? OnDownloadProgressChanged;

        public static string GetSteamCMDPath()
        {
            if (!SteamSetup.IsSteamInstalled())
                throw new InvalidOperationException("SteamCMD is not installed.");

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamcmd", "steamcmd.exe");
        }

        private static void ParseSteamCMDResponse(string? data, ref GeneralSteamResponse response)
        {
            if (string.IsNullOrEmpty(data))
                return;


            if (data.Contains("This account is protected by") ||
                data.Contains("Please confirm the login"))
            {
                response.response = ClientResponse.AUTHENABLED;
            }
            else if (data.Contains("(Invalid Password)"))
            {
                response.response = ClientResponse.INVALIDPASSWORD;
            }
        }

        public static async Task<GeneralSteamResponse> DownloadGMOD(LoginDetails login)
        {
            EnsureSteamSetup();

            if (SteamSetup.IsSteamCMDRunning())
                throw new InvalidOperationException("Another instance of SteamCMD is already running.");

            var generalResponse = new GeneralSteamResponse
            {
                Data = null,
                Progress = null,
                response = ClientResponse.SUCCESSFUL
            };

            var psi = new ProcessStartInfo
            {
                FileName = GetSteamCMDPath(),
                Arguments = $"+force_install_dir \"{Application.StartupPath}\\GMOD\\\" " +
                            $"+login {login.Username} {login.Password} " +
                            $"+app_update 4000 " +
                            $"+quit",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };


            process.OutputDataReceived += (_, e) => ParseSteamCMDResponse(e.Data, ref generalResponse);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            process.CancelOutputRead();
            process.CancelErrorRead();

            return generalResponse;
        }

        public static async Task<GeneralSteamResponse> DoesClientHave2FA(
            LoginDetails login,
            bool sandboxed = false,
            Sandboxie? sandbox = null)
        {
            var generalResponse = new GeneralSteamResponse
            {
                Data = null,
                Progress = null,
                response = ClientResponse.SUCCESSFUL
            };

            if (sandboxed)
            {
                if (sandbox is null)
                    throw new ArgumentNullException(nameof(sandbox), "Sandbox argument cannot be null.");

                string args = $"+login {sandbox._client.LoginDetails.Username} " +
                              $"{sandbox._client.LoginDetails.Password} +quit";

                var result = SandboxieWrapper.RunBoxedWithRedirect(
                    GetSteamCMDPath(),
                    args,
                    sandbox._sandboxName,
                    line => ParseSteamCMDResponse(line, ref generalResponse));

                if (result.Result && result.Data != null)
                    await result.Data.WaitForExitAsync();
            }
            else
            {
                var psi = new ProcessStartInfo
                {
                    FileName = GetSteamCMDPath(),
                    Arguments = $"+login {login.Username} {login.Password} +quit",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

                process.OutputDataReceived += (_, e) =>
                {
                    Debug.WriteLine($"[OUT] {e.Data}");
                    ParseSteamCMDResponse(e.Data, ref generalResponse);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                process.CancelOutputRead();
                process.CancelErrorRead();
            }

            return generalResponse;
        }

        private static void EnsureSteamSetup()
        {
            if (!SteamSetup.IsSteamInstalled())
                throw new InvalidOperationException("SteamCMD is not installed.");
        }
    }
}
