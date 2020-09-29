using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CustomSSDTMigrationScripts
{
    public class JsonSettingsProvider : ISettingsProvider
    {
        private const string SETTINGS_FILENAME = "ssdt.migration.scripts.json";

        public Settings GetSettings(string directory)
        {
            var settings = new Settings();

            if (!Directory.Exists(directory))
            {
                var error = $"JsonSettingsProvider: The directory ${directory} does not exist";
                throw new DirectoryNotFoundException(error);
            }

            var settingFiles = Directory.GetFiles(directory, SETTINGS_FILENAME, SearchOption.AllDirectories);
            if (settingFiles.Count() == 0)
            {
                var warning = $"JsonSettingsProvider: A settings file ssdt.migration.scripts.json does not exist under the directory {directory} (recursively). Default values will be used.";
                Logger.LogWarning(warning);
            }
            else if (settingFiles.Count() > 1)
            {
                var error = $"JsonSettingsProvider: Multiple setting files ssdt.migration.scripts.json found under the directory {directory} (recursively)";
                throw new InvalidOperationException(error);
            }
            else
            {
                Logger.LogMessage($"JsonSettingsProvider: Found settings file {settingFiles.First()}");
                var settingsContent = File.ReadAllText(settingFiles.First());
                settings = JsonSerializer.Deserialize<Settings>(settingsContent);
                Logger.LogMessage($"JsonSettingsProvider: Settings successfully deserialized.{Environment.NewLine}{settingsContent}");
            }

            return settings;
        }
    }
}