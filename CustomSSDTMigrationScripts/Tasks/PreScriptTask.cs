using System.IO;

namespace CustomSSDTMigrationScripts
{
    public class PreScriptTask : PrePostScriptTaskBase
    {
        public PreScriptTask()
        {

        }

        public override string ScriptTypeName => ScriptTypes.PreScript;

        public override ScriptSettings DefaultScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Path.Combine(base.ProjectRootDirectory, "Scripts", "PreScripts"),
            ScriptNamePattern = @"^(\d{14})_(.*).sql",
            ScriptRecursiveSearch = true,
            GeneratedScriptPath = Path.Combine(base.ProjectRootDirectory, "Scripts", "RunPreScriptsGenerated.sql"),
            ExecutionFilterMode = ScriptExecutionFilterMode.FILTER_BY_ALL,
            ExecutionFilterValue = null,
            TreatScriptNamePatternMismatchAsError = true,
            TreatHashMismatchAsError = true
        };

        public override ScriptSettings CurrentScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = settings.PreScripts.ScriptBaseDirectory ?? DefaultScriptSettings.ScriptBaseDirectory,
            ScriptNamePattern = settings.PreScripts.ScriptNamePattern ?? DefaultScriptSettings.ScriptNamePattern,
            ScriptRecursiveSearch = settings.PreScripts.ScriptRecursiveSearch ?? DefaultScriptSettings.ScriptRecursiveSearch,
            GeneratedScriptPath = settings.PreScripts.GeneratedScriptPath ?? DefaultScriptSettings.GeneratedScriptPath,
            ExecutionFilterMode = settings.PreScripts.ExecutionFilterMode ?? DefaultScriptSettings.ExecutionFilterMode,
            ExecutionFilterValue = settings.PreScripts.ExecutionFilterValue ?? DefaultScriptSettings.ExecutionFilterValue,
            TreatScriptNamePatternMismatchAsError = settings.PreScripts.TreatScriptNamePatternMismatchAsError ?? DefaultScriptSettings.TreatScriptNamePatternMismatchAsError,
            TreatHashMismatchAsError = settings.PreScripts.TreatHashMismatchAsError ?? DefaultScriptSettings.TreatHashMismatchAsError,
        };
    }
}
