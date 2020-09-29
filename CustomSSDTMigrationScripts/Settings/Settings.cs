using System.Text.Json.Serialization;

namespace CustomSSDTMigrationScripts
{
    public class Settings
    {
        public Settings()
        {
            PreScripts = new ScriptSettings();
            PostScripts = new ScriptSettings();
            ReferenceDataScripts = new ScriptSettings();
        }

        [JsonPropertyName("PreScripts")]
        public ScriptSettings PreScripts { get; set; }

        [JsonPropertyName("PostScripts")]
        public ScriptSettings PostScripts { get; set; }

        [JsonPropertyName("ReferenceDataScripts")]
        public ScriptSettings ReferenceDataScripts { get; set; }
    }
}