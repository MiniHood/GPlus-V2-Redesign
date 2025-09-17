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
            await sandboxie.InitialiseAsync(details);
            Sandboxies.Append(sandboxie);
        }

        private static async Task UnregisterSandbox(Sandboxie sandboxie)
        {
            Sandboxies.Remove(sandboxie);
        }

        public static async Task DeleteSandbox(Sandboxie sandboxie)
        {
            // Deleting a sandboxie is as simple as deleting the folder in C:\Sandbox\{sandboxname} and removing the entry from the ini file
            // Sandboxie has an option to auto delete sandboxes on close, but this is not reliable so we'll do it ourselves
            Settings CurrentSettings = SettingsManager.CurrentSettings;
            if (CurrentSettings == null)
                throw new Exception("Settings not loaded correctly.");
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = CurrentSettings.General.SandboxieBoxCreator,
                Arguments = $"delete {sandboxie.SandboxName}",
                UseShellExecute = false,
                CreateNoWindow = false
            };
            using (Process proc = Process.Start(startInfo))
            {
                proc.WaitForExit();
            }
            await UnregisterSandbox(sandboxie);
        }

        public static async Task OnShutdown()
        {
            foreach (var sandbox in Sandboxies)
            {
                await DeleteSandbox(sandbox);
            }
        }

        public static Sandboxie CreateNewSandbox(LoginDetails loginDetails)
        {
            // Null check rq
            Settings CurrentSettings = SettingsManager.CurrentSettings;
            if (CurrentSettings == null)
                throw new Exception("Settings not loaded correctly.");

            // Lets now create the actual sandboxie, only way to do it is either through files or letting the sandboxie creator do it for us, probably safer to give it to teh creator
            // Steam doesn't allow 2 of the same usernames so we'll use that as the sandbox name
            // Remember kids, creating a box doesn't actually launch anything, it just writes to the ini file

            List<string> SandboxieArguments = new List<string>
            {
                SettingsManager.CurrentSettings.BoxCreation.ConfigLevel,
                SettingsManager.CurrentSettings.BoxCreation.Enabled,
                SettingsManager.CurrentSettings.BoxCreation.BoxName,
                SettingsManager.CurrentSettings.BoxCreation.PromptForFileMigration,
                SettingsManager.CurrentSettings.BoxCreation.Template,
                SettingsManager.CurrentSettings.BoxCreation.AutoDelete,
                SettingsManager.CurrentSettings.BoxCreation.AutoRecover,
                SettingsManager.CurrentSettings.BoxCreation.CopyLimitKb,
                SettingsManager.CurrentSettings.BoxCreation.Template,
            };


            foreach (var arg in SandboxieArguments)
            {
                if (arg == null)
                    throw new Exception("One or more Sandboxie box creation settings are null, please check your settings file.");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = CurrentSettings.General.SandboxieBoxCreator,
                    Arguments = $"set {loginDetails.Username} {arg}",
                    UseShellExecute = false,
                    CreateNoWindow = false // could turn this on but then the user wouldn't get any noticable feedback, users may find it more satisfying to see the window pop up
                };

                using (Process proc = Process.Start(startInfo))
                {
                    proc.WaitForExit();
                }
            }


            Sandboxie NewSandbox = new Sandboxie(loginDetails.Username);
            RegisterSandbox(NewSandbox, loginDetails);
            return NewSandbox;
        }
    }
}
