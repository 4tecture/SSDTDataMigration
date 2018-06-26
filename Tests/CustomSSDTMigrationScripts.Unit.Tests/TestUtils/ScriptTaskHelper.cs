using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;

namespace CustomSSDTMigrationScripts.Unit.Tests.TestUtils
{
    public static class ScriptTaskHelper
    {
        private const string SETTINGS_FILENAME = "ssdt.migration.scripts.json";

        public static PostScriptTask GetPostScriptTask(string rootDir)
        {
            var task = new PostScriptTask()
            {
                ProjectRootDirectory = rootDir
            };
            task.Initialize();

            return task;
        }

        public static PreScriptTask GetPreScriptTask(string rootDir)
        {
            var task = new PreScriptTask()
            {
                ProjectRootDirectory = rootDir
            };
            task.Initialize();

            return task;
        }

        public static ReferenceDataScriptTask GetReferenceDataScriptTask(string rootDir)
        {
            var task = new ReferenceDataScriptTask()
            {
                ProjectRootDirectory = rootDir
            };
            task.Initialize();

            return task;
        }

        public static void RemoveTestSettings(string directory)
        {
            Directory.GetFiles(directory, SETTINGS_FILENAME, SearchOption.AllDirectories).ToList().ForEach(f => File.Delete(f));
        }

        public static void CreateTestSettings(string directory, Settings settings = null)
        {
            settings = settings ?? new Settings();

            var settingsContent = JsonConvert.SerializeObject(settings);
            File.WriteAllText(Path.Combine(directory, SETTINGS_FILENAME), settingsContent, Encoding.UTF8);
        }
    }
}
