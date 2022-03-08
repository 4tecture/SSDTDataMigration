using System.IO;

namespace CustomSSDTMigrationScripts
{
    public class PreScriptTask : PrePostScriptTaskBase
    {
        public PreScriptTask()
        {
        }

        public override string ScriptTypeName => ScriptTypes.PreScript;

        protected override ScriptSettings DefaultScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Path.Combine(ProjectRootDirectory, "Scripts", "PreScripts"),
            ScriptNamePattern = @"^(\d{14})_(.*).sql",
            ScriptRecursiveSearch = true,
            GeneratedScriptPath = Path.Combine(ProjectRootDirectory, "Scripts", "RunPreScriptsGenerated.sql"),
            ExecutionFilterMode = ScriptExecutionFilterMode.FILTER_BY_ALL,
            ExecutionFilterValue = null,
            TreatScriptNamePatternMismatchAsError = true,
            TreatHashMismatchAsError = true
        };

        public override ScriptSettings ScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Settings.PreScripts.ScriptBaseDirectory ?? DefaultScriptSettings.ScriptBaseDirectory,
            ScriptNamePattern = Settings.PreScripts.ScriptNamePattern ?? DefaultScriptSettings.ScriptNamePattern,
            ScriptRecursiveSearch = Settings.PreScripts.ScriptRecursiveSearch ?? DefaultScriptSettings.ScriptRecursiveSearch,
            GeneratedScriptPath = Settings.PreScripts.GeneratedScriptPath ?? DefaultScriptSettings.GeneratedScriptPath,
            ExecutionFilterMode = Settings.PreScripts.ExecutionFilterMode ?? DefaultScriptSettings.ExecutionFilterMode,
            ExecutionFilterValue = Settings.PreScripts.ExecutionFilterValue ?? DefaultScriptSettings.ExecutionFilterValue,
            TreatScriptNamePatternMismatchAsError = Settings.PreScripts.TreatScriptNamePatternMismatchAsError ?? DefaultScriptSettings.TreatScriptNamePatternMismatchAsError,
            TreatHashMismatchAsError = Settings.PreScripts.TreatHashMismatchAsError ?? DefaultScriptSettings.TreatHashMismatchAsError,
        };
    }
}
