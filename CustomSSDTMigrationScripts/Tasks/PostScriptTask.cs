using System.IO;

namespace CustomSSDTMigrationScripts
{
    public class PostScriptTask : PrePostScriptTaskBase
    {
        public PostScriptTask()
        {

        }

        public override string ScriptTypeName => ScriptTypes.PostScript;

        public override ScriptSettings DefaultScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Path.Combine(base.ProjectRootDirectory, "Scripts", "PostScripts"),
            ScriptNamePattern = @"^(\d{14})_(.*).sql",
            ScriptRecursiveSearch = true,
            GeneratedScriptPath = Path.Combine(base.ProjectRootDirectory, "Scripts", "RunPostScriptsGenerated.sql"),
            ExecutionFilterMode = ScriptExecutionFilterMode.FILTER_BY_ALL,
            ExecutionFilterValue = null,
            TreatScriptNamePatternMismatchAsError = true,
            TreatHashMismatchAsError = true
        };

        public override ScriptSettings CurrentScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = settings.PostScripts.ScriptBaseDirectory ?? DefaultScriptSettings.ScriptBaseDirectory,
            ScriptNamePattern = settings.PostScripts.ScriptNamePattern ?? DefaultScriptSettings.ScriptNamePattern,
            ScriptRecursiveSearch = settings.PostScripts.ScriptRecursiveSearch ?? DefaultScriptSettings.ScriptRecursiveSearch,
            GeneratedScriptPath = settings.PostScripts.GeneratedScriptPath ?? DefaultScriptSettings.GeneratedScriptPath,
            ExecutionFilterMode = settings.PostScripts.ExecutionFilterMode ?? DefaultScriptSettings.ExecutionFilterMode,
            ExecutionFilterValue = settings.PostScripts.ExecutionFilterValue ?? DefaultScriptSettings.ExecutionFilterValue,
            TreatScriptNamePatternMismatchAsError = settings.PostScripts.TreatScriptNamePatternMismatchAsError ?? DefaultScriptSettings.TreatScriptNamePatternMismatchAsError,
            TreatHashMismatchAsError = settings.PostScripts.TreatHashMismatchAsError ?? DefaultScriptSettings.TreatHashMismatchAsError,
        };
    }
}
