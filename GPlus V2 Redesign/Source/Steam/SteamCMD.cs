using GPlus.Source.Enums;
using GPlus.Source.Sandboxing;
using GPlus.Source.Structs;
using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static SandboxieWrapper;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace GPlus.Source.Steam
{
    internal class SteamCMD
    {
        public static event EventHandler<int>? OnDownloadProgressChanged;

        public static string GetSteamCMDPath()
        {
            if(!SteamSetup.IsSteamInstalled())
                throw new Exception("SteamCMD is not installed.");

            return AppDomain.CurrentDomain.BaseDirectory + "steamcmd\\steamcmd.exe";
        }

        public async static Task<ClientResponse> DownloadGMOD(LoginDetails login)
        {
            return ClientResponse.SUCCESSFUL;

            if(!SteamSetup.IsSteamInstalled())
                throw new Exception("SteamCMD is not installed.");

            if(SteamSetup.IsSteamCMDRunning())
                throw new Exception("Another instance of SteamCMD is already running.");

            ClientResponse response = ClientResponse.SUCCESSFUL; // Default to successful unless an error is found

            var psi = new ProcessStartInfo
            {
                FileName = SteamCMD.GetSteamCMDPath(),
                Arguments = $"+force_install_dir \"{Application.StartupPath}\\GMOD\\\" +login {login.Username} {login.Password} +app_update 4000 +quit",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.WriteLine($"[OUT] {e.Data}");

                    if (e.Data.Contains("This account is protected by") ||
                        e.Data.Contains("Please confirm the login"))
                    {
                        response = ClientResponse.AUTHENABLED;
                    }
                    else if (e.Data.Contains("(Invalid Password)"))
                    {
                        response = ClientResponse.INVALIDPASSWORD;
                    }
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync().ContinueWith(t =>
            {
                process.CancelOutputRead();
                process.CancelErrorRead();
                process.Close();
            });
        }

        public static async Task<ClientResponse> DoesClientHave2FA(LoginDetails login,
            bool Sandboxed = false,
            Sandboxie _sandbox = null)
        {
            ClientResponse response = ClientResponse.SUCCESSFUL;

            switch (Sandboxed)
            {
                case true:
                    if (_sandbox == null)
                        throw new Exception("Sandbox argument was null");

                    string args = $"+login {_sandbox._client.LoginDetails.Username}" +
                                  $" {_sandbox._client.LoginDetails.Password}" +
                                  $" +quit";

                    var result = RunBoxedWithRedirect(
                        GetSteamCMDPath(),
                        args,
                        _sandbox._sandboxName,
                        line =>
                        {
                            if (string.IsNullOrEmpty(line))
                                return;

                            if (line.Contains("This account is protected by") ||
                                line.Contains("Please confirm the login"))
                            {
                                response = ClientResponse.AUTHENABLED;
                            }
                            else if (line.Contains("(Invalid Password)"))
                            {
                                response = ClientResponse.INVALIDPASSWORD;
                            }
                        });

                    if (result.Result && result.Data != null)
                        await result.Data.WaitForExitAsync();

                    break;

                case false:
                    var psi = new ProcessStartInfo
                    {
                        FileName = SteamCMD.GetSteamCMDPath(),
                        Arguments = $"+login {login.Username} {login.Password} +quit",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Debug.WriteLine($"[OUT] {e.Data}");

                            if (e.Data.Contains("This account is protected by") ||
                                e.Data.Contains("Please confirm the login"))
                            {
                                response = ClientResponse.AUTHENABLED;
                            }
                            else if (e.Data.Contains("(Invalid Password)"))
                            {
                                response = ClientResponse.INVALIDPASSWORD;
                            }
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await process.WaitForExitAsync().ContinueWith(t =>
                    {
                        process.CancelOutputRead();
                        process.CancelErrorRead();
                        process.Close();
                    });

                    break;
            }

            return response;
        }

    }
}
