using GPlus.Source.General;
using GPlus.Source.Structs;
using System.Diagnostics;

namespace GPlus.Source.Sandboxing
{
    internal class SandboxieManager
    {
        public static List<Sandboxie> Sandboxies = new List<Sandboxie>();

        public static int GetFreePort()
        {
            Random rand = new Random();
            int port;

            do
            {
                port = rand.Next(10000, 60000);
            }
            while (Sandboxies.Cast<Sandboxie>().Any(s => s.Client.RCONPort == port));

            return port;
        }

        private static async void RegisterSandbox(Sandboxie sandboxie, LoginDetails details)
        {
            Sandboxies.Add(sandboxie);
            await sandboxie.InitialiseAsync(details);
        }

        private static async Task UnregisterSandbox(Sandboxie sandboxie)
        {
            Sandboxies.Remove(sandboxie);
        }

        public static async Task DeleteSandbox(Sandboxie sandboxie)
        {
            Settings CurrentSettings = SettingsManager.CurrentSettings;
            if (CurrentSettings == null)
                throw new Exception("Settings not loaded correctly.");

            var processesResult = SandboxieWrapper.GetBoxedProcesses(sandboxie.SandboxName);
            if (processesResult.Data != null)
            {
                foreach (var proc in processesResult.Data)
                {
                    try
                    {
                        if (!proc.HasExited)
                        {
                            proc.Kill();
                            await proc.WaitForExitAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[Sandboxie] Failed to kill process {proc.Id}: {ex.Message}");
                    }
                }
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = CurrentSettings.General.SandboxieBoxCreator,
                Arguments = $"delete {sandboxie.SandboxName}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var proc = Process.Start(startInfo))
            {
                await proc.WaitForExitAsync();
            }

                SandboxieWrapper.RemoveBox(sandboxie.SandboxName);

            await UnregisterSandbox(sandboxie);
        }


        public static async Task OnShutdown()
        {
            var queue = new Queue<Sandboxie>(Sandboxies);

            while (queue.Count > 0)
            {
                var sandbox = queue.Dequeue();
                await DeleteSandbox(sandbox);
            }
        }

        public static async Task<Sandboxie> CreateNewSandboxAsync(LoginDetails loginDetails)
        {
            // Null check rq
            Settings currentSettings = SettingsManager.CurrentSettings
                ?? throw new Exception("Settings not loaded correctly.");

            // Build arguments list
            List<string> sandboxieArguments = new List<string>
            {
                currentSettings.BoxCreation.ConfigLevel,
                currentSettings.BoxCreation.Enabled,
                currentSettings.BoxCreation.BoxName,
                currentSettings.BoxCreation.PromptForFileMigration,
                currentSettings.BoxCreation.Template,
                currentSettings.BoxCreation.AutoDelete,
                currentSettings.BoxCreation.AutoRecover,
                currentSettings.BoxCreation.CopyLimitKb,
                currentSettings.BoxCreation.Template,
            };

            foreach (var arg in sandboxieArguments)
            {
                if (arg == null)
                    throw new Exception("One or more Sandboxie box creation settings are null, please check your settings file.");

                var startInfo = new ProcessStartInfo
                {
                    FileName = currentSettings.General.SandboxieBoxCreator,
                    Arguments = $"set {loginDetails.Username} {arg}",
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                using (var proc = Process.Start(startInfo))
                {
                    if (proc != null)
                        await proc.WaitForExitAsync(); // async instead of blocking
                }
            }

            var newSandbox = new Sandboxie(loginDetails.Username);
            RegisterSandbox(newSandbox, loginDetails);
            return newSandbox;
        }

    }
}
