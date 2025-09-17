using GPlus.Source.Enums;
using GPlus.Source.Sandboxing;
using GPlus.Source.Structs;
using System.Diagnostics;

namespace GPlus.Source.Steam
{
    internal class SteamCMD
    {
        public static event EventHandler<int>? OnDownloadProgressChanged;
        public static Process CurrentSteamCMDInstance { get; private set; } = null!;


        public static bool IsSteamCMDRunning()
        {
            if (CurrentSteamCMDInstance != null)
                return true;
            return false;
        }

        public static string GetSteamCMDPath()
        {
            if (!SteamSetup.IsSteamInstalled())
                throw new InvalidOperationException("SteamCMD is not installed.");

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamcmd", "steamcmd.exe");
        }

        public static void ForceStopSteamCMD() // Extremely unsafe
        {
            if (CurrentSteamCMDInstance == null)
                throw new Exception("Force steam cmd shutdown failed");

            CurrentSteamCMDInstance.Kill(true);
            // Clear logs to avoid recovery loop
            try
            {
                string steamlogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamcmd", "logs");
                Directory.Delete(steamlogs, true);
                Directory.CreateDirectory(steamlogs);
            }
            catch (Exception ex) { }
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

            if (IsSteamCMDRunning())
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
            CurrentSteamCMDInstance = process;

            process.OutputDataReceived += (_, e) => ParseSteamCMDResponse(e.Data, ref generalResponse);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            process.CancelOutputRead();
            process.CancelErrorRead();
            CurrentSteamCMDInstance = null!;

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

                string args = $"+login {sandbox.Client.LoginDetails.Username} " +
                              $"{sandbox.Client.LoginDetails.Password} +quit";

                var result = SandboxieWrapper.RunBoxedWithRedirect(
                    GetSteamCMDPath(),
                    args,
                    sandbox.SandboxName,
                    line => ParseSteamCMDResponse(line, ref generalResponse));

                CurrentSteamCMDInstance = result.Data;

                if (result.Result && result.Data != null)
                {
                    await result.Data.WaitForExitAsync();
                    CurrentSteamCMDInstance = null!;
                }
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
                CurrentSteamCMDInstance = process;
                process.OutputDataReceived += (_, e) =>
                {
                    Debug.WriteLine($"[OUT] {e.Data}");
                    ParseSteamCMDResponse(e.Data, ref generalResponse);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();
                CurrentSteamCMDInstance = null!;

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
