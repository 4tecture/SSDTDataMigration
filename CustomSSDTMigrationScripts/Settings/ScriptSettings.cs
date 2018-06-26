using Newtonsoft.Json;

namespace CustomSSDTMigrationScripts
{
    public class ScriptSettings
    {
        [JsonProperty("ScriptBaseDirectory")]
        public string ScriptBaseDirectory { get; set; }

        [JsonProperty("ScriptNamePattern")]
        public string ScriptNamePattern { get; set; }

        [JsonProperty("ScriptRecursiveSearch")]
        public bool? ScriptRecursiveSearch { get; set; }

        [JsonProperty("GeneratedScriptPath")]
        public string GeneratedScriptPath { get; set; }

        [JsonProperty("ExecutionFilterMode")]
        public string ExecutionFilterMode { get; set; }

        [JsonProperty("ExecutionFilterValue")]
        public string ExecutionFilterValue { get; set; }

        [JsonProperty("TreatScriptNamePatternMismatchAsError")]
        public bool? TreatScriptNamePatternMismatchAsError { get; set; }

        [JsonProperty("TreatHashMismatchAsError")]
        public bool? TreatHashMismatchAsError { get; set; }
    }
}
