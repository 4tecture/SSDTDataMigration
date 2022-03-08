using System.IO;

namespace CustomSSDTMigrationScripts
{
    public class PostScriptTask : PrePostScriptTaskBase
    {
        public PostScriptTask()
        {
        }

        public override string ScriptTypeName => ScriptTypes.PostScript;

        protected override ScriptSettings DefaultScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Path.Combine(ProjectRootDirectory, "Scripts", "PostScripts"),
            ScriptNamePattern = @"^(\d{14})_(.*).sql",
            ScriptRecursiveSearch = true,
            GeneratedScriptPath = Path.Combine(ProjectRootDirectory, "Scripts", "RunPostScriptsGenerated.sql"),
            ExecutionFilterMode = ScriptExecutionFilterMode.FILTER_BY_ALL,
            ExecutionFilterValue = null,
            TreatScriptNamePatternMismatchAsError = true,
            TreatHashMismatchAsError = true
        };

        public override ScriptSettings ScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Settings.PostScripts.ScriptBaseDirectory ?? DefaultScriptSettings.ScriptBaseDirectory,
            ScriptNamePattern = Settings.PostScripts.ScriptNamePattern ?? DefaultScriptSettings.ScriptNamePattern,
            ScriptRecursiveSearch = Settings.PostScripts.ScriptRecursiveSearch ?? DefaultScriptSettings.ScriptRecursiveSearch,
            GeneratedScriptPath = Settings.PostScripts.GeneratedScriptPath ?? DefaultScriptSettings.GeneratedScriptPath,
            ExecutionFilterMode = Settings.PostScripts.ExecutionFilterMode ?? DefaultScriptSettings.ExecutionFilterMode,
            ExecutionFilterValue = Settings.PostScripts.ExecutionFilterValue ?? DefaultScriptSettings.ExecutionFilterValue,
            TreatScriptNamePatternMismatchAsError = Settings.PostScripts.TreatScriptNamePatternMismatchAsError ?? DefaultScriptSettings.TreatScriptNamePatternMismatchAsError,
            TreatHashMismatchAsError = Settings.PostScripts.TreatHashMismatchAsError ?? DefaultScriptSettings.TreatHashMismatchAsError,
        };
    }
}
