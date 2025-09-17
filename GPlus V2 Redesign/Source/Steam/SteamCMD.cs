using GPlus.Source.Enums;
using GPlus.Source.Sandboxing;
using GPlus.Source.Structs;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GPlus.Source.Steam
{
    internal class SteamCMD
    {
        public static event EventHandler<GeneralSteamResponse>? OnSteamCMDResponseUpdated;
        public static Process CurrentSteamCMDInstance = null;


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

        public async static Task WaitForSteamCMDExit()
        {
            if (CurrentSteamCMDInstance == null)
                throw new Exception("No SteamCMD instance to wait for.");
            await CurrentSteamCMDInstance.WaitForExitAsync();
            CurrentSteamCMDInstance = null;
        }

        private static void ParseSteamCMDResponse(string? data, ref GeneralSteamResponse response)
        {
            if (string.IsNullOrEmpty(data))
                return;

            response.responseType = ResponseType.Unknown;

            if (data.Contains("Looks like steam didn't shutdown cleanly, scheduling immediate update check"))
                response.response = ClientResponse.RETRY;

            if (data.Contains("This account is protected by") || data.Contains("Please confirm the login"))
                response.response = ClientResponse.AUTHENABLED;
            else if (data.Contains("(Invalid Password)"))
                response.response = ClientResponse.INVALIDPASSWORD;
            else if (data.Contains("Success!"))
                response.response = ClientResponse.SUCCESSFUL;

            if (data.ToLower().Contains("verifying"))
                response.responseType = ResponseType.Verifying;
            else if (data.ToLower().Contains("downloading"))
                response.responseType = ResponseType.Downloading;
            else if (data.ToLower().Contains("commiting"))
                response.responseType = ResponseType.Commiting;

            var progressMatch = Regex.Match(data, @"progress:\s*(\d+(\.\d+)?)", RegexOptions.IgnoreCase);
            if (progressMatch.Success && double.TryParse(progressMatch.Groups[1].Value, out double progressValue))
                response.Progress = (int)Math.Round(progressValue);

            Debug.WriteLine($"Response type: {response.responseType}, Progress: {response.Progress}");
            Debug.WriteLine($"Data: {data}");

            OnSteamCMDResponseUpdated?.Invoke(null, response);
        }

        public static async Task<GeneralSteamResponse> DownloadGarrysMod(LoginDetails login)
        {
            EnsureSteamSetup();

            if (IsSteamCMDRunning())
                throw new InvalidOperationException("Another instance of SteamCMD is already running.");

            GeneralSteamResponse generalResponse = new()
            {
                Data = null,
                Progress = null,
                response = ClientResponse.SUCCESSFUL
            };

            bool retry;
            do
            {
                retry = false;

                var psi = new ProcessStartInfo
                {
                    FileName = GetSteamCMDPath(),
                    Arguments = $"+force_install_dir \"{Application.StartupPath}SteamCMD\\GMOD\" " +
                                $"+login {login.Username} {login.Password} " +
                                $"+app_update 4000 validate " +
                                $"+quit",
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
                    ParseSteamCMDResponse(e.Data, ref generalResponse);
                    if (generalResponse.response == ClientResponse.RETRY)
                        retry = true;

                    OnSteamCMDResponseUpdated?.Invoke(null, generalResponse);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (retry)
                {
                    Debug.WriteLine("[SteamCMD] Retry requested. Shutting down and retrying...");
                    await WaitForSteamCMDExit(); // ensure SteamCMD fully exits before retry
                }

                process.CancelOutputRead();
                process.CancelErrorRead();
                CurrentSteamCMDInstance = null!;

            } while (retry);

            return generalResponse;
        }

        public static async Task<GeneralSteamResponse> DoesClientHave2FA(LoginDetails login)
        {
            GeneralSteamResponse generalResponse = new()
            {
                Data = null,
                Progress = null,
                response = ClientResponse.SUCCESSFUL
            };

            bool retry;
            do
            {
                retry = false;
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

                var tcs = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (_, e) =>
                {
                    if (e.Data == null) return;
                    ParseSteamCMDResponse(e.Data, ref generalResponse);
                    if (generalResponse.response == ClientResponse.RETRY)
                        retry = true;
                };

                process.Exited += (_, _) => tcs.TrySetResult(true);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for either the process to exit or 10 seconds to elapse
                var timeoutTask = Task.Delay(10000);
                var finishedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                if (finishedTask == timeoutTask)
                {
                    Debug.WriteLine("[SteamCMD] Timeout exceeded, assuming login details are incorrect.");
                    try { process.Kill(); } catch { }
                    generalResponse.response = ClientResponse.INVALIDPASSWORD;
                    retry = false;
                }
                else if (retry)
                {
                    Debug.WriteLine("[SteamCMD] Retry requested. Shutting down and retrying...");
                    await WaitForSteamCMDExit();
                }

                CurrentSteamCMDInstance = null!;

            } while (retry);

            return generalResponse;
        }



        private static void EnsureSteamSetup()
        {
            if (!SteamSetup.IsSteamInstalled())
                throw new InvalidOperationException("SteamCMD is not installed.");
        }
    }
}
