using CustomSSDTMigrationScripts.Unit.Tests.TestData.Scenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomSSDTMigrationScripts.Unit.Tests
{
    [TestClass]
    public class PostScriptTests
    {
        private string MsBuildExe => Context.Properties["MSBuildExe"].ToString();

        private string SSDTTestProjectRootDir => Context.Properties["SSDTTestProjectRootDir"].ToString();

        private static TestContext Context { get; set; }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Context = context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        //[TestMethod]
        public void GetScripts_PostScripUsestDefaultSettings_ExpectAll()
        {
            // Arrange
            var scenarioManager = new ScenarioManager(SSDTTestProjectRootDir, MsBuildExe);

            var test = scenarioManager.Scenario1_SchemeMigrationOnly();

            // Act

            // Assert
        }
    }
}