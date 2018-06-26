using CustomSSDTMigrationScripts.Unit.Tests.TestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSSDTMigrationScripts.Unit.Tests.ScriptBaseTaskTests
{
    [TestClass]
    public class BaseTaskTests
    {
        public string TempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        private ScriptSamples sampels = new ScriptSamples();

        [TestInitialize]
        public void TestInitialize()
        {
            Directory.CreateDirectory(TempFolder);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Directory.Delete(TempFolder, true);
        }

        [TestMethod]
        public void GetScripts_PostScripUsestDefaultSettings_ExpectAll()
        {
            // Arrange
            var task = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(task.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Act
            var scripts = task.GetScripts();

            // Assert
            Assert.AreEqual(sampels.ValidPrePostSamplesRecursiveCount, scripts.Count);
        }

        [TestMethod]
        public void GetScripts_PreScripUsetDefaultSettings_ExpectAll()
        {
            // Arrange
            var task = ScriptTaskHelper.GetPreScriptTask(TempFolder);
            CreateSampleScripts(task.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Act
            var scripts = task.GetScripts();

            // Assert
            Assert.AreEqual(sampels.ValidPrePostSamplesRecursiveCount, scripts.Count);
        }

        [TestMethod]
        public void GetScripts_ReferenceDataUsestDefaultSettings_ExpectAll()
        {
            // Arrange
            var task = ScriptTaskHelper.GetReferenceDataScriptTask(TempFolder);
            CreateSampleScripts(task.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidRefDataSamples);

            // Act
            var scripts = task.GetScripts();

            // Assert
            Assert.AreEqual(sampels.ValidRefDataSamplesRecursiveCount, scripts.Count);
        }

        [TestMethod]
        public void GetScripts_NonRecursiveSetting_ExpectOnlyRootFolderScripts()
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings { PostScripts = new ScriptSettings { ScriptRecursiveSearch = false } });
            var task = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(task.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);


            // Act
            var scripts = task.GetScripts();

            // Assert
            Assert.AreEqual(sampels.ValidPrePostSamplesRootCount, scripts.Count);
        }

        [TestMethod]
        public void GetScripts_TreatScriptNamePatternMismatchAsError_ThrowException()
        {
            // Arrange
            var preTask = ScriptTaskHelper.GetPreScriptTask(TempFolder);
            var postTask = ScriptTaskHelper.GetPreScriptTask(TempFolder);
            var refDataTask = ScriptTaskHelper.GetPreScriptTask(TempFolder);

            CreateSampleScripts(preTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.InvalidPrePostSamples);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.InvalidPrePostSamples);
            CreateSampleScripts(refDataTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.InvalidRefDataSamples);

            // Assert
            Assert.ThrowsException<FormatException>(() => preTask.GetScripts());
            Assert.ThrowsException<FormatException>(() => postTask.GetScripts());
            Assert.ThrowsException<FormatException>(() => refDataTask.GetScripts());
        }

        [TestMethod]
        public void GetScripts_TreatScriptNamePatternMismatchAsWarning_ReturnValidScriptsOnly()
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { TreatScriptNamePatternMismatchAsError = false },
                PreScripts = new ScriptSettings { TreatScriptNamePatternMismatchAsError = false },
                ReferenceDataScripts = new ScriptSettings { TreatScriptNamePatternMismatchAsError = false }
            });

            var preTask = ScriptTaskHelper.GetPreScriptTask(TempFolder);
            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            var refDataTask = ScriptTaskHelper.GetReferenceDataScriptTask(TempFolder);

            CreateSampleScripts(preTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.InvalidPrePostSamples);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.InvalidPrePostSamples);
            CreateSampleScripts(refDataTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.InvalidRefDataSamples);

            // Assert
            Assert.AreEqual(sampels.ValidPrePostSamplesRecursiveCount, preTask.GetScripts().Count);
            Assert.AreEqual(sampels.ValidPrePostSamplesRecursiveCount, postTask.GetScripts().Count);
            Assert.AreEqual(sampels.ValidRefDataSamplesRecursiveCount, refDataTask.GetScripts().Count);
        }

        [TestMethod]
        public void GetScripts_GlobalScriptNamePatternRecursive_ReturnAllScripts()
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ScriptNamePattern = "(.*)" }, // Global match pattern
                ReferenceDataScripts = new ScriptSettings { ScriptNamePattern = "(.*)" } // Global match pattern
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            var refDataTask = ScriptTaskHelper.GetReferenceDataScriptTask(TempFolder);

            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.InvalidPrePostSamples);
            CreateSampleScripts(refDataTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.InvalidRefDataSamples);

            // Assert
            Assert.AreEqual(sampels.InvalidPrePostSamples.Count(), postTask.GetScripts().Count);
            Assert.AreEqual(sampels.InvalidRefDataSamples.Count(), refDataTask.GetScripts().Count);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(7, 7)]
        [DataRow(1000, 7)]
        public void GetScripts_ByCountRecursive_ExpectScriptCountAndValidNewestScript(int filterValue, int expectedScriptCount)
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ExecutionFilterMode = "count", ExecutionFilterValue = filterValue.ToString(), ScriptRecursiveSearch = true },
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Act
            var scripts = postTask.GetScripts();

            // Assert
            Assert.AreEqual(expectedScriptCount, scripts.Count);
            if (expectedScriptCount > 0)
            {
                Assert.AreEqual(sampels.ValidPrePostSamplesLatestRecursive, scripts.Last().Name);
            }
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(3, 3)]
        [DataRow(1000, 3)]
        public void GetScripts_ByCountNoneRecursive_ExpectScriptCountAndValidNewestScript(int filterValue, int expectedScriptCount)
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ExecutionFilterMode = "count", ExecutionFilterValue = filterValue.ToString(), ScriptRecursiveSearch = false },
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Act
            var scripts = postTask.GetScripts();

            // Assert
            Assert.AreEqual(expectedScriptCount, scripts.Count);
            if (expectedScriptCount > 0)
            {
                Assert.AreEqual(sampels.ValidPrePostSamplesLatestRoot, scripts.Last().Name);
            }
        }

        [TestMethod]
        [DataRow("20170101010101", 7)]
        [DataRow("20200101020202", 0)]
        [DataRow("20180227020202", 2)]
        public void GetScripts_ByDateRecursive_ExpectScriptCountAndValidNewestScript(string fromDate, int expectedScriptCount)
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ExecutionFilterMode = "date", ExecutionFilterValue = fromDate, ScriptRecursiveSearch = true },
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Act
            var scripts = postTask.GetScripts();

            // Assert
            Assert.AreEqual(expectedScriptCount, scripts.Count);
            if (expectedScriptCount > 0)
            {
                Assert.AreEqual(sampels.ValidPrePostSamplesLatestRecursive, scripts.Last().Name);
            }
        }

        [TestMethod]
        [DataRow("20170101010101", 3)]
        [DataRow("20200101020202", 0)]
        [DataRow("20180227020202", 1)]
        public void GetScripts_ByDateNoneRecursive_ExpectScriptCountAndValidNewestScript(string fromDate, int expectedScriptCount)
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ExecutionFilterMode = "date", ExecutionFilterValue = fromDate, ScriptRecursiveSearch = false },
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Act
            var scripts = postTask.GetScripts();

            // Assert
            Assert.AreEqual(expectedScriptCount, scripts.Count);
            if (expectedScriptCount > 0)
            {
                Assert.AreEqual(sampels.ValidPrePostSamplesLatestRoot, scripts.Last().Name);
            }
        }

        [TestMethod]
        [DataRow("20180115")]           // missing time
        [DataRow("201801151400")]       // missing seconds
        [DataRow("20180115140010555")]  // milliseconds
        [DataRow("20180132140010")]     // invalid date
        public void GetScripts_ByDateWithInvalidFormat_ThrowFormatException(string fromDate)
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ExecutionFilterMode = "date", ExecutionFilterValue = fromDate, ScriptRecursiveSearch = true },
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Assert
            Assert.ThrowsException<FormatException>(() => postTask.GetScripts());
        }

        [TestMethod]
        [DataRow(100000, 7)]
        [DataRow(0, 0)]
        public void GetScripts_ByDaysRecursive_ExpectScriptCountAndValidNewestScript(double days, int expectedScriptCount)
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ExecutionFilterMode = "days", ExecutionFilterValue = days.ToString(), ScriptRecursiveSearch = true },
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Act
            var scripts = postTask.GetScripts();

            // Assert
            Assert.AreEqual(expectedScriptCount, scripts.Count);
            if (expectedScriptCount > 0)
            {
                Assert.AreEqual(sampels.ValidPrePostSamplesLatestRecursive, scripts.Last().Name);
            }
        }

        [TestMethod]
        [DataRow(100000, 3)]
        [DataRow(0, 0)]
        public void GetScripts_ByDaysNoneRecursive_ExpectScriptCountAndValidNewestScript(double days, int expectedScriptCount)
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ExecutionFilterMode = "days", ExecutionFilterValue = days.ToString(), ScriptRecursiveSearch = false },
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Act
            var scripts = postTask.GetScripts();

            // Assert
            Assert.AreEqual(expectedScriptCount, scripts.Count);
            if (expectedScriptCount > 0)
            {
                Assert.AreEqual(sampels.ValidPrePostSamplesLatestRoot, scripts.Last().Name);
            }
        }

        [TestMethod]
        public void GetScripts_ByUnknownFilterMode_ThrowException()
        {
            // Arrange
            ScriptTaskHelper.CreateTestSettings(TempFolder, new Settings
            {
                PostScripts = new ScriptSettings { ExecutionFilterMode = "invalid", ExecutionFilterValue = "invalid" },
            });

            var postTask = ScriptTaskHelper.GetPostScriptTask(TempFolder);
            CreateSampleScripts(postTask.CurrentScriptSettings.ScriptBaseDirectory, sampels.ValidPrePostSamples);

            // Assert
            Assert.ThrowsException<ArgumentException>(() => postTask.GetScripts());
        }

        private void CreateSampleScripts(string dir, List<string> sampleFiles)
        {
            // Cleanup and recreate sample files
            if (Directory.Exists(dir)) Directory.Delete(dir, true);

            sampleFiles.ForEach(f =>
            {
                FileInfo fi = new FileInfo(Path.Combine(dir, f));
                if (!fi.Directory.Exists)
                    fi.Directory.Create();
                using (var fileStream = fi.Create())
                {
                }
            });
        }
    }
}
