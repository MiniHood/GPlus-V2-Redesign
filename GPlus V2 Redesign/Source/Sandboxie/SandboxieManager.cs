using GPlus_V2_Redesign.GUI.Elements;
using System.Diagnostics;

namespace GPlus_V2_Redesign.Source.Sandboxie
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
            while (Sandboxies.Cast<Sandboxie>().Any(s => s._client.RCONPort == port));

            return port;
        }

        private static void RegisterSandbox(Sandboxie sandboxie)
        {
            Sandboxies.Append(sandboxie);
        }

        public static Sandboxie CreateNewSandbox(LoginDetails loginDetails)
        {
            // Null check rq
            Settings CurrentSettings = SettingsManager.CurrentSettings;
            if (CurrentSettings == null)
                throw new Exception("Settings not loaded correctly.");

            // honestly probably not needed but whats a howniceofyou release without unnessacery code and shitty spelling
            string CleanedName = new string(loginDetails.UncleanedUsername.Where(c => char.IsLetterOrDigit(c)).ToArray());
            if (string.IsNullOrWhiteSpace(CleanedName))
                CleanedName = $"GPlusUser{new Random().Next(1000, 9999)}";

            Sandboxie? ExistingSandbox = Sandboxies.FirstOrDefault(s => s._sandboxName == CleanedName);
            if (ExistingSandbox != null)
            {
                throw new Exception($"A sandbox with the name {CleanedName} already exists.");
            }

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
                    Arguments = $"set {CleanedName} {arg}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }


            Sandboxie NewSandbox = new Sandboxie(new LoginDetails { CleanedUsername = CleanedName, UncleanedUsername = loginDetails.UncleanedUsername, Password = loginDetails.Password});
            RegisterSandbox(NewSandbox);
            return NewSandbox;
        }
    }
}
