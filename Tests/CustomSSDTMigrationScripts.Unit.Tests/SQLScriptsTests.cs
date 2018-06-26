//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.IO;
//using System.Text;

//namespace CustomSSDTMigrationScripts.Unit.Tests
//{
//    [TestClass]
//    public class SQLScriptsTests
//    {
//        public string TempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

//        [TestInitialize]
//        public void TestInitialize()
//        {
//            Directory.CreateDirectory(TempFolder);
//            InitFiles();
//        }

//        [TestCleanup]
//        public void TestCleanup()
//        {
//            Directory.Delete(TempFolder, true);
//        }

//        [TestMethod]
//        public void CreatePreMigrationScripts_Simple()
//        {
//            // Arrange
//            var logbuffer = new StringBuilder();
//            var task = CreateBuildTaskPre(logbuffer);

//            // Act
//            var buildResult = task.Execute();
//            var resultingScript = GetGeneratedFilePre();

//            // Assert
//            Assert.IsTrue(buildResult);
//            Assert.IsTrue(resultingScript.Contains("asdf"));

//        }

//        private string GetGeneratedFilePre()
//        {
//            return GetGeneratedFile("Pre");
//        }

//        private string GetGeneratedFilePost()
//        {
//            return GetGeneratedFile("Post");
//        }

//        private string GetGeneratedFile(object prefix)
//        {
//            return File.ReadAllText(Path.Combine(TempFolder, "Scripts", $"Run{prefix}Scripts"));
//        }

//        private void InitFiles()
//        {
//            Directory.CreateDirectory(Path.Combine(TempFolder, "Scripts", "PreScripts"));
//            File.Create(Path.Combine(TempFolder, "Scripts", "PreScripts", $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_Sample1.sql"));
//            File.Create(Path.Combine(TempFolder, "Scripts", "PreScripts", $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_Sample2.sql"));
//            File.Create(Path.Combine(TempFolder, "Scripts", "PreScripts", $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_Sample3.sql"));

//            Directory.CreateDirectory(Path.Combine(TempFolder, "Scripts", "PostScripts"));
//            File.Create(Path.Combine(TempFolder, "Scripts", "PostScripts", $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_Sample1.sql"));
//            File.Create(Path.Combine(TempFolder, "Scripts", "PostScripts", $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_Sample2.sql"));
//            File.Create(Path.Combine(TempFolder, "Scripts", "PostScripts", $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_Sample3.sql"));
//        }

//        private AddCustomSSDTMigrationScripts CreateBuildTaskPost(StringBuilder logBuffer = null)
//        {
//            return CreateBuildTask("Post", logBuffer);
//        }

//        private AddCustomSSDTMigrationScripts CreateBuildTaskPre(StringBuilder logBuffer = null)
//        {
//            return CreateBuildTask("Pre", logBuffer);
//        }

//        private AddCustomSSDTMigrationScripts CreateBuildTask(string prefix, StringBuilder logBuffer = null)
//        {
//            return new AddCustomSSDTMigrationScripts() { BaseDirectory = Path.Combine(TempFolder, "Scripts"), ScriptDirectory = Path.Combine(TempFolder, "Scripts", $"{prefix}Scripts"), TargetFile = Path.Combine(TempFolder, "Scripts", $"Run{prefix}Scripts"), LogBuffer = logBuffer };
//        }


//    }
//}
