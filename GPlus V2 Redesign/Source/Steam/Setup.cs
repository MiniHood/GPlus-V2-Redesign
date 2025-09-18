using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace GPlus.Source.Steam
{
    public static class SteamSetup
    {
        private const string SteamCMDUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

        public static event EventHandler<int>? OnDownloadProgressChanged;
        public static event EventHandler<int>? OnZipProgressChanged;
        public static event EventHandler<int>? OnSteamCMDUpdateProgressChanged;
        public static event EventHandler? OnSteamSetupCompleted;

        public static bool IsSteamInstalled() =>
            File.Exists(Path.Combine("SteamCMD", "steamcmd.exe"));

        public static bool IsGMODInstalled() =>
            Directory.Exists(Path.Combine("SteamCMD", "gmod"));

        public static async Task<bool> UnzipSteamClient()
        {
            return await Task.Run(() =>
            {
                string zipPath = Path.Combine("SteamCMD", "steamcmd.zip");
                if (!File.Exists(zipPath))
                    throw new FileNotFoundException("SteamCMD zip not found.", zipPath);

                using var archive = ZipFile.OpenRead(zipPath);
                int totalEntries = archive.Entries.Count;
                int processedEntries = 0;

                foreach (var entry in archive.Entries)
                {
                    string destinationPath = Path.Combine("SteamCMD", entry.FullName);

                    if (string.IsNullOrEmpty(entry.Name)) // folder entry
                    {
                        Directory.CreateDirectory(destinationPath);
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                    entry.ExtractToFile(destinationPath, overwrite: true);

                    processedEntries++;
                    int percent = (int)((double)processedEntries / totalEntries * 100);
                    OnZipProgressChanged?.Invoke(null, percent);
                }

                return true;
            });
        }

        public static async Task<bool> DownloadSteamClient()
        {
            Directory.CreateDirectory("SteamCMD");

            Debug.WriteLine("Downloading SteamCMD...");
            using var client = new HttpClient();
            using var response = await client.GetAsync(SteamCMDUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            string zipPath = Path.Combine("SteamCMD", "steamcmd.zip");
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None);

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            long totalRead = 0;
            var buffer = new byte[8192];

            while (true)
            {
                int read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
                if (read == 0) break;

                await fileStream.WriteAsync(buffer.AsMemory(0, read));
                totalRead += read;

                if (totalBytes > 0)
                {
                    int progress = (int)((double)totalRead / totalBytes * 100);
                    OnDownloadProgressChanged?.Invoke(null, progress);
                    Debug.WriteLine($"Download progress: {progress}%");
                }
            }

            Debug.WriteLine("Download complete.");
            return true;
        }

        public static async Task<bool> AllowSteamUpdate()
        {
            if (!IsSteamInstalled() || SteamCMD.IsSteamCMDRunning())
                return false;

            bool retry;
            do
            {
                retry = false;

                var psi = new ProcessStartInfo
                {
                    FileName = SteamCMD.GetSteamCMDPath(),
                    Arguments = "+login anonymous +exit",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
                SteamCMD.CurrentSteamCMDInstance = process;

                process.OutputDataReceived += (_, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data)) return;

                    Debug.WriteLine($"[OUT] [AllowSteamUpdate] {e.Data}");

                    // Parse progress
                    var match = Regex.Match(e.Data, @"\[\s*(\d+)%\]");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int progress) && progress > 0)
                    {
                        OnSteamCMDUpdateProgressChanged?.Invoke(null, progress);
                        Debug.WriteLine($"[AllowSteamUpdate] Progress: {progress}%");
                    }

                    // Check for retry signal
                    if (e.Data.Contains("Retry") || e.Data.Contains("Please try again"))
                    {
                        retry = true;
                    }
                };

                process.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine($"[ERR] [AllowSteamUpdate] {e.Data}");
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (retry)
                {
                    Debug.WriteLine("[SteamCMD] Retry requested. Shutting down and retrying...");
                    await SteamCMD.WaitForSteamCMDExit(); // ensure SteamCMD fully exits before retry
                }

                process.CancelOutputRead();
                process.CancelErrorRead();
                SteamCMD.CurrentSteamCMDInstance = null;

            } while (retry);

            OnSteamSetupCompleted?.Invoke(null, EventArgs.Empty);
            return true;
        }

    }
}
