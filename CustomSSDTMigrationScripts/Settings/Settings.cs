using Newtonsoft.Json;

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

        [JsonProperty("PreScripts")]
        public ScriptSettings PreScripts { get; set; }
        [JsonProperty("PostScripts")]
        public ScriptSettings PostScripts { get; set; }
        [JsonProperty("ReferenceDataScripts")]
        public ScriptSettings ReferenceDataScripts { get; set; }
    }
}
