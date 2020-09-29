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
        protected Settings settings;

        public ScriptBaseTask()
        {
            // Assign msbuild logger to internal logger
            Logger.logger = base.Log;
        }

        public string ProjectRootDirectory { get; set; }

        public abstract string ScriptTypeName { get; }
        public abstract ScriptSettings DefaultScriptSettings { get; }
        public abstract ScriptSettings CurrentScriptSettings { get; }
        public abstract void ExecuteScriptTask();

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

        public virtual void Initialize()
        {
            // read migration settings
            settings = settingsProvider.GetSettings(ProjectRootDirectory);

            // delete generated scripts
            if (File.Exists(CurrentScriptSettings.GeneratedScriptPath))
            {
                File.Delete(CurrentScriptSettings.GeneratedScriptPath);
                Logger.LogMessage($"Generated script file \"{CurrentScriptSettings.GeneratedScriptPath}\" has been deleted.");
            }
        }

        public List<Script> GetScripts()
        {
            var scripts = new List<Script>();
            var sqlScripts = new List<FileInfo>();
            var scriptSearchOption = CurrentScriptSettings.ScriptRecursiveSearch.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            // Get scripts
            if (!Directory.Exists(CurrentScriptSettings.ScriptBaseDirectory))
            {
                Logger.LogMessage($"The script base directory {CurrentScriptSettings.ScriptBaseDirectory} does not exist.");
                return scripts;
            }

            sqlScripts = Directory.GetFiles(CurrentScriptSettings.ScriptBaseDirectory, $"*.{SQL_SCRIPT_EXTENSION}", scriptSearchOption)
                .Select(s => new FileInfo(s)).ToList();
            Logger.LogMessage($"Found total {sqlScripts.Count} sql scripts. Start validating and filtering scripts...");

            // Validate by naming pattern
            sqlScripts = FilterScriptsByNamingPattern(sqlScripts);
            Logger.LogMessage($"Filtered ");
            scripts = ExtractScriptInformation(sqlScripts);
            scripts = FilterScriptsByExecutionMode(scripts);

            return scripts.OrderBy(i => i.OrderCriteria).ToList();
        }

        private List<Script> FilterScriptsByExecutionMode(List<Script> scripts)
        {
            // Order by descending, newest scripts first
            switch (CurrentScriptSettings.ExecutionFilterMode.ToLower())
            {
                case ScriptExecutionFilterMode.FILTER_BY_COUNT:
                    var filterCount = int.Parse(CurrentScriptSettings.ExecutionFilterValue);
                    if (filterCount < 0)
                    {
                        throw new ArgumentException($"Invalid execution filter value (count): {filterCount}");
                    }

                    scripts = scripts.OrderByDescending(s => s.OrderCriteria).Take(filterCount).ToList();
                    break;
                case ScriptExecutionFilterMode.FILTER_BY_DAYS:
                    var filterDays = double.Parse(CurrentScriptSettings.ExecutionFilterValue);
                    if (filterDays < 0)
                    {
                        throw new ArgumentException($"Invalid execution filter value (days): {filterDays}");
                    }

                    scripts = scripts.Where(s => DateTime.ParseExact(s.OrderCriteria, "yyyyMMddHHmmss", CultureInfo.InvariantCulture) >= DateTime.Now.AddDays(-filterDays).Date).ToList();
                    break;
                case ScriptExecutionFilterMode.FILTER_BY_DATE:
                    var filterDate = DateTime.ParseExact(CurrentScriptSettings.ExecutionFilterValue, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    scripts = scripts.Where(s => DateTime.ParseExact(s.OrderCriteria, "yyyyMMddHHmmss", CultureInfo.InvariantCulture) >= filterDate).ToList();
                    break;
                case ScriptExecutionFilterMode.FILTER_BY_ALL:
                    // no filtering applied, take all scripts
                    break;
                default:
                    throw new ArgumentException($"Unknown execution filter mode {CurrentScriptSettings.ExecutionFilterMode}");
            }

            // Order by ascending, newest scripts last
            return scripts;
        }

        private List<Script> ExtractScriptInformation(List<FileInfo> sqlScripts)
        {
            var validScripts = new List<Script>();
            foreach (var sqlScript in sqlScripts)
            {
                var match = Regex.Match(sqlScript.Name, CurrentScriptSettings.ScriptNamePattern);

                // Extract order criteria
                var orderCriteria = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(orderCriteria))
                {
                    throw new ArgumentNullException($"The order criteria part cannot be extracted from {sqlScript.Name}");
                }

                // Create new script object
                validScripts.Add(new Script
                {
                    UniqueScriptId = sqlScript.FullName.Replace($@"{new FileInfo(CurrentScriptSettings.ScriptBaseDirectory).Directory.FullName}\", ""),
                    Name = sqlScript.Name,
                    OrderCriteria = orderCriteria,
                    MigrationType = ScriptTypeName,
                    FullFilePath = sqlScript.FullName
                });
            }

            return validScripts;
        }

        private List<FileInfo> FilterScriptsByNamingPattern(List<FileInfo> sqlScripts)
        {
            var validScripts = new List<FileInfo>();
            foreach (var sqlScript in sqlScripts)
            {
                // Verify script naming convention
                var match = Regex.Match(sqlScript.Name, CurrentScriptSettings.ScriptNamePattern);
                if (!match.Success && CurrentScriptSettings.TreatScriptNamePatternMismatchAsError.Value)
                {
                    throw new FormatException($"The sql script {sqlScript.FullName} does not meet the naming pattern {CurrentScriptSettings.ScriptNamePattern}.");
                }
                else if (!match.Success)
                {
                    Logger.LogWarning($"The sql script {sqlScript.FullName} does not meet the naming pattern {CurrentScriptSettings.ScriptNamePattern} and will be ignored.");
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
