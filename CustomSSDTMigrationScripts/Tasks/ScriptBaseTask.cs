using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CustomSSDTMigrationScripts
{
    public abstract class ScriptBaseTask : Microsoft.Build.Utilities.Task
    {
        private const string SQL_SCRIPT_EXTENSION = "sql";
        private readonly ISettingsProvider settingsProvider = new JsonSettingsProvider();

        public ScriptBaseTask()
        {
            // Assign msbuild logger to internal logger
            Logger.logger = base.Log;
        }

        protected Settings Settings { get; private set; }

        public string ProjectRootDirectory { get; set; }

        public abstract string ScriptTypeName { get; }

        protected abstract ScriptSettings DefaultScriptSettings { get; }

        public abstract ScriptSettings ScriptSettings { get; }

        protected abstract void ExecuteScriptTask();

        public override bool Execute()
        {
            try
            {
                Initialize();

                // execute custom task
                ExecuteScriptTask();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return false;
            }
        }

        public void Initialize(bool cleanup = true)
        {
            // read migration settings
            Settings = settingsProvider.GetSettings(ProjectRootDirectory);

            // delete generated scripts
            if (cleanup)
            {
                if (File.Exists(ScriptSettings.GeneratedScriptPath))
                {
                    File.Delete(ScriptSettings.GeneratedScriptPath);
                    Logger.LogMessage($"Generated script file \"{ScriptSettings.GeneratedScriptPath}\" has been deleted.");
                }
            }
        }

        public IEnumerable<Script> GetScripts()
        {
            return GetScripts(ScriptSettings);
        }

        public IEnumerable<Script> GetScripts(ScriptSettings scriptSettings)
        {
            var scripts = new List<Script>();
            if (scriptSettings is not null)
            {
                var scriptSearchOption = scriptSettings.ScriptRecursiveSearch.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                // Get scripts
                if (!Directory.Exists(scriptSettings.ScriptBaseDirectory))
                {
                    Logger.LogMessage($"The script base directory {scriptSettings.ScriptBaseDirectory} does not exist.");
                    return scripts;
                }

                var sqlScripts = Directory.GetFiles(scriptSettings.ScriptBaseDirectory, $"*.{SQL_SCRIPT_EXTENSION}", scriptSearchOption).Select(s => new FileInfo(s)).ToList();
                if (sqlScripts.Any())
                {
                    Logger.LogMessage($"Found total {sqlScripts.Count} sql scripts. Start validating and filtering scripts...");
                }
                else
                {
                    Logger.LogWarning($"Found no sql scripts in {scriptSettings.ScriptBaseDirectory}.");
                }

                // Validate by naming pattern
                sqlScripts = FilterScriptsByNamingPattern(scriptSettings, sqlScripts);
                Logger.LogMessage($"Filtered ");
                scripts = ExtractScriptInformation(scriptSettings, sqlScripts);
                scripts = FilterScriptsByExecutionMode(scriptSettings, scripts);
            }

            return scripts.OrderBy(i => i.OrderCriteria).ToList();
        }

        private List<Script> FilterScriptsByExecutionMode(ScriptSettings scriptSettings, List<Script> scripts)
        {
            var filteredScripts = new List<Script>(scripts);
            // Order by descending, newest scripts first
            switch (scriptSettings.ExecutionFilterMode.ToLower())
            {
                case ScriptExecutionFilterMode.FILTER_BY_COUNT:
                    var filterCount = int.Parse(scriptSettings.ExecutionFilterValue);
                    if (filterCount < 0)
                    {
                        throw new ArgumentException($"Invalid execution filter value (count): {filterCount}");
                    }

                    filteredScripts = filteredScripts.OrderByDescending(s => s.OrderCriteria).Take(filterCount).ToList();
                    break;
                case ScriptExecutionFilterMode.FILTER_BY_DAYS:
                    var filterDays = double.Parse(scriptSettings.ExecutionFilterValue);
                    if (filterDays < 0)
                    {
                        throw new ArgumentException($"Invalid execution filter value (days): {filterDays}");
                    }

                    filteredScripts = filteredScripts.Where(s => DateTime.ParseExact(s.OrderCriteria, "yyyyMMddHHmmss", CultureInfo.InvariantCulture) >= DateTime.Now.AddDays(-filterDays).Date).ToList();
                    break;
                case ScriptExecutionFilterMode.FILTER_BY_DATE:
                    var filterDate = DateTime.ParseExact(scriptSettings.ExecutionFilterValue, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    filteredScripts = filteredScripts.Where(s => DateTime.ParseExact(s.OrderCriteria, "yyyyMMddHHmmss", CultureInfo.InvariantCulture) >= filterDate).ToList();
                    break;
                case ScriptExecutionFilterMode.FILTER_BY_ALL:
                    // no filtering applied, take all scripts
                    break;
                default:
                    throw new ArgumentException($"Unknown execution filter mode {scriptSettings.ExecutionFilterMode}");
            }

            // Order by ascending, newest scripts last
            return filteredScripts;
        }

        private List<Script> ExtractScriptInformation(ScriptSettings scriptSettings, List<FileInfo> sqlScripts)
        {
            var validScripts = new List<Script>();
            foreach (var sqlScript in sqlScripts)
            {
                var match = Regex.Match(sqlScript.Name, scriptSettings.ScriptNamePattern);

                // Extract order criteria
                var orderCriteria = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(orderCriteria))
                {
                    throw new ArgumentNullException($"The order criteria part cannot be extracted from {sqlScript.Name}");
                }

                var uniqueScriptId = sqlScript.FullName
                    .Replace(new FileInfo(scriptSettings.ScriptBaseDirectory).Directory.FullName, string.Empty)
                    .Replace("/", @"\")
                    .Substring(1);

                // Create new script object
                validScripts.Add(new Script
                {
                    UniqueScriptId = uniqueScriptId,
                    Name = sqlScript.Name,
                    OrderCriteria = orderCriteria,
                    MigrationType = ScriptTypeName,
                    FullFilePath = sqlScript.FullName
                });
            }

            return validScripts;
        }

        private List<FileInfo> FilterScriptsByNamingPattern(ScriptSettings scriptSettings, List<FileInfo> sqlScripts)
        {
            var validScripts = new List<FileInfo>();
            foreach (var sqlScript in sqlScripts)
            {
                // Verify script naming convention
                var match = Regex.Match(sqlScript.Name, scriptSettings.ScriptNamePattern);
                if (!match.Success && scriptSettings.TreatScriptNamePatternMismatchAsError.Value)
                {
                    throw new FormatException($"The sql script {sqlScript.FullName} does not meet the naming pattern {scriptSettings.ScriptNamePattern}.");
                }
                else if (!match.Success)
                {
                    Logger.LogWarning($"The sql script {sqlScript.FullName} does not meet the naming pattern {scriptSettings.ScriptNamePattern} and will be ignored.");
                }
                else
                {
                    validScripts.Add(sqlScript);
                }
            }

            return validScripts;
        }
    }
}
