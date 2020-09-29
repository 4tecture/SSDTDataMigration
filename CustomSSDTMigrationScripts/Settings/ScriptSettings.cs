using System.Text.Json.Serialization;

namespace CustomSSDTMigrationScripts
{
    public class ScriptSettings
    {
        [JsonPropertyName("ScriptBaseDirectory")]
        public string ScriptBaseDirectory { get; set; }

        [JsonPropertyName("ScriptNamePattern")]
        public string ScriptNamePattern { get; set; }

        [JsonPropertyName("ScriptRecursiveSearch")]
        public bool? ScriptRecursiveSearch { get; set; }

        [JsonPropertyName("GeneratedScriptPath")]
        public string GeneratedScriptPath { get; set; }

        [JsonPropertyName("ExecutionFilterMode")]
        public string ExecutionFilterMode { get; set; }

        [JsonPropertyName("ExecutionFilterValue")]
        public string ExecutionFilterValue { get; set; }

        [JsonPropertyName("TreatScriptNamePatternMismatchAsError")]
        public bool? TreatScriptNamePatternMismatchAsError { get; set; }

        [JsonPropertyName("TreatHashMismatchAsError")]
        public bool? TreatHashMismatchAsError { get; set; }
    }
}