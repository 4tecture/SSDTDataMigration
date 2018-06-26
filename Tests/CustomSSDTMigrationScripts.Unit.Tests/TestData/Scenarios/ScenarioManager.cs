using System.Diagnostics;
using System.IO;

namespace CustomSSDTMigrationScripts.Unit.Tests.TestData.Scenarios
{
    public class ScenarioManager
    {
        private readonly string ssdtTestProjectDir;
        private readonly string msbuildExe;

        public ScenarioManager(string ssdtTestProjectDir, string msbuildExe)
        {
            this.ssdtTestProjectDir = Path.GetFullPath(ssdtTestProjectDir);
            this.msbuildExe = Path.GetFullPath(msbuildExe);
        }

        public string TargetsFilePath => Path.Combine(ssdtTestProjectDir, "Scripts.targets");
        public string ProjectFilePath => Path.Combine(ssdtTestProjectDir, "CustomSSDTMigrationScripts.Integration.Tests.sqlproj");
        public string ProjectOutDir => Path.Combine(ssdtTestProjectDir, @"out_scenario1");

        public string Scenario1_SchemeMigrationOnly()
        {
            File.WriteAllText(TargetsFilePath,
$@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
    <ItemGroup>
    <Build Include=""{ssdtTestProjectDir}\Tables\_MigrationScriptsHistory.sql""/>
    <Build Include=""{ssdtTestProjectDir}\Tables\Customer.sql""/>
    </ItemGroup>
</Project>");


            // Use ProcessStartInfo class.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = msbuildExe;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = $"{ProjectFilePath} /p:Configuration=Release /p:OutDir={ProjectOutDir}";

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }

            return null;
        }
    }
}
