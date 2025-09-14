using Newtonsoft.Json;
using System;
using System.IO;

namespace GPlus_V2_Redesign.Source
{
    public class Settings
    {
        public GeneralSettings General { get; set; } = new GeneralSettings();
        public SandboxieBoxCreation BoxCreation { get; set; } = new SandboxieBoxCreation();

        public class GeneralSettings
        {
            public string? SandboxiePath { get; set; } = null;
            public string? SteamPath { get; set; } = null;
            public string? GMODDirectory { get; set; } = null;
            public string? GMODExecutable { get; set; } = null;
            public string? SandboxieBoxCreator { get; set; } = null;
            public string? GMODLaunchArguments { get; set; } = null;
            public string? RCONPassword { get; set; } = null;
            public string? CommunicationDLLPath { get; set; } = null;
            public string? SteamAPIKey { get; set; } = null;

        }

        public class SandboxieBoxCreation
        {
            public string? ConfigLevel { get; set; } = null;
            public string? Enabled { get; set; } = null;
            public string? AutoDelete { get; set; } = null;
            public string? BoxName { get; set; } = null;
            public string? AutoRecover { get; set; } = null;
            public string? PromptForFileMigration { get; set; } = null;
            public string? CopyLimitKb { get; set; } = null;
            public string? Template { get; set; } = null;
        }
    }

    public class SettingsManager
    {
        public static Settings CurrentSettings { get; private set; } = new Settings();

        static void CreateSettings()
        {
            if (!Directory.Exists("Settings"))
                Directory.CreateDirectory("Settings");

            var defaults = new Settings
            {
                General = new Settings.GeneralSettings
                {
                    SandboxiePath = @"C:\Program Files\Sandboxie-Plus\",
                    SteamPath = @"D:\Steam\steam.exe",
                    GMODDirectory = @"D:\Steam\steamapps\common\GarrysMod",
                    GMODExecutable = "gmod.exe",
                    SandboxieBoxCreator = @"C:\Program Files\Sandboxie-Plus\SbieIni.exe",
                    GMODLaunchArguments =
                        "-console -novid -nohltv -nosteamcontroller -nosound -nojoy -noipx " +
                        "-noshaderapi -particles 1 -nopix -nopreload -nod3d9ex -low -textmode " +
                        "-heapsize 262144 -disallowhwmorph -high -reflectionTextureSize 0 " +
                        "-soft -softparticlesdefaultoff",
                    RCONPassword = "changeme",
                    CommunicationDLLPath = $"{Application.StartupPath}\\Resources\\Communication.dll",
                    SteamAPIKey = "changeme",
                },
                BoxCreation = new Settings.SandboxieBoxCreation
                {
                    ConfigLevel = "ConfigLevel 7",
                    Enabled = "Enabled y",
                    AutoDelete = "AutoDelete n",
                    BoxName = "BoxNameTitle y",
                    AutoRecover = "AutoRecover y",
                    PromptForFileMigration = "PromptForFileMigration n",
                    CopyLimitKb = "CopyLimitKb 50000000",
                    Template = "Template AutoRecoverIgnore"
                }
            };

            File.WriteAllText("Settings\\settings.json",
                JsonConvert.SerializeObject(defaults, Formatting.Indented));

            if (!File.Exists("Settings\\settings.json"))
                throw new Exception("Failed to create settings file");
        }

        public static Settings LoadSettings()
        {
            if (!File.Exists("Settings\\settings.json"))
                CreateSettings();

            string fileContents = File.ReadAllText("Settings\\settings.json");
            Settings settings = JsonConvert.DeserializeObject<Settings>(fileContents) ?? new Settings();
            CurrentSettings = settings;
            return settings;
        }

        public static void ChangeSettings(Settings newSettings)
        {
            CurrentSettings = newSettings;
            File.WriteAllText("Settings\\settings.json",
                JsonConvert.SerializeObject(newSettings, Formatting.Indented));
            LoadSettings();
        }
    }
}
