using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace GPlus.Source.Steam
{
    public static class SteamSetup
    {
        private const string SteamCMDUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
        private static int DownloadProgress = 0;

        public static event EventHandler<int>? OnDownloadProgressChanged;
        public static event EventHandler<int>? OnZipProgressChanged;
        public static event EventHandler<int>? SteamCMDUpdateProgressChange;

        public static event EventHandler? OnSteamSetupCompleted;

        public static bool IsSteamInstalled()
        {
            return System.IO.File.Exists("SteamCMD\\steamcmd.exe");
        }

        public async static Task<bool> UnzipSteamClient()
        {
            return await Task.Run(() =>
            {
                using (ZipArchive archive = ZipFile.OpenRead("SteamCMD\\steamcmd.zip"))
                {
                    int total = archive.Entries.Count;
                    int current = 0;

                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine("SteamCMD", entry.FullName);

                        // Ensure folder exists
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(destinationPath);
                            continue;
                        }

                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                        entry.ExtractToFile(destinationPath, true);

                        current++;
                        int percent = (int)((double)current / total * 100);

                        OnZipProgressChanged?.Invoke(null, percent);
                    }
                }

                return true;
            });
        }

        public async static Task<bool> DownloadSteamClient()
        {
            // https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip

            Directory.CreateDirectory("SteamCMD");
            Debug.WriteLine("Downloading SteamCMD...");
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(SteamCMDUrl, HttpCompletionOption.ResponseHeadersRead))
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (FileStream fileStream = new FileStream("SteamCMD\\steamcmd.zip", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var totalRead = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;
                Debug.WriteLine($"Total bytes to download: {totalBytes}");
                do
                {
                    var read = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        Debug.WriteLine("Download complete.");
                        isMoreToRead = false;
                        return true;
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;

                        if (totalBytes != -1)
                        {
                            double progress = (double)totalRead / totalBytes * 100;
                            DownloadProgress = (int)progress;
                            OnDownloadProgressChanged?.Invoke(null, DownloadProgress);
                            Debug.WriteLine($"Download progress: {DownloadProgress}%");
                        }
                    }
                }
                while (isMoreToRead);
            }

            return false;
        }

        public static async Task<bool> AllowSteamUpdate()
        {
            if (!IsSteamInstalled())
                return false;

            var psi = new ProcessStartInfo
            {
                FileName = "SteamCMD\\steamcmd.exe",
                Arguments = "+login anonymous +exit +@sSteamCmdForcePlatformType windows",
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

                    var match = Regex.Match(e.Data, @"\[\s*(\d+)%\]");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int progress))
                    {
                        SteamCMDUpdateProgressChange?.Invoke(null, progress);
                        Debug.WriteLine($"SteamCMD Update Progress: {progress}%");
                    }
                }
            };


            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.WriteLine($"[ERR] {e.Data}");
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            return true;
        }

    }
}
