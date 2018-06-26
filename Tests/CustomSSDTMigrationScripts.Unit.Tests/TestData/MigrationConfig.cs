using System.Collections.Generic;

namespace CustomSSDTMigrationScripts.Unit.Tests.TestData
{
    public class MigrationConfig
    {
        private List<string> Functions;
        private List<string> PreScripts;
        private List<string> PostScripts;
        private List<string> ReferenceDataScripts;
        private List<string> StoredProcedures;
        private List<string> Tables;
        private List<string> UserDefinedDataTypes;
        private List<string> Views;

        public MigrationConfig()
        {
            Functions = new List<string>();
            PreScripts = new List<string>();
            PostScripts = new List<string>();
            ReferenceDataScripts = new List<string>();
            StoredProcedures = new List<string>();
            Tables = new List<string>();
            UserDefinedDataTypes = new List<string>();
            Views = new List<string>();
        }

        public void RegisterFunction(string function)
        {
            if (!Functions.Contains(function)) Functions.Add(function);
        }

        public void RegisterPreScripts(string preScript)
        {
            if (!PreScripts.Contains(preScript)) PreScripts.Add(preScript);
        }

//        public string GenerateTargets(string rootDir)
//        {
//            File.WriteAllText(TargetsFilePath,
//$@"<?xml version=""1.0"" encoding=""utf-8""?>
//<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
//    <ItemGroup>
//    <Build Include=""{ssdtTestProjectDir}\Tables\_MigrationScriptsHistory.sql""/>
//    <Build Include=""{ssdtTestProjectDir}\Tables\Customer.sql""/>
//    </ItemGroup>
//</Project>");
//        }
    }
}
