using System.Collections.Generic;
using System.Linq;

namespace CustomSSDTMigrationScripts.Unit.Tests.TestUtils
{
    public class ScriptSamples
    {
        public ScriptSamples()
        {
        }

        // ===========================================================================
        // Valid: PRE / POST
        // ===========================================================================
        public int ValidPrePostSamplesRootCount => 3;
        public int ValidPrePostSamplesRecursiveCount => ValidPrePostSamples.Count();
        public string ValidPrePostSamplesLatestRoot => "20180228150101_Script6.sql";
        public string ValidPrePostSamplesLatestRecursive => "20180504070000_Script7.sql";
        public IEnumerable<string> ValidPrePostSamples => new List<string>
        {
            @"20180102124055_Script1.sql",
            @"20180102124100_Script2.sql",
            @"subfolder2/20180202081120_Script5.sql",
            $@"subfolder2/{ValidPrePostSamplesLatestRecursive}",
            $@"{ValidPrePostSamplesLatestRoot}",
            @"subfolder1/20180102130000_Script3.sql",
            @"subfolder1/20180102130002_Script4.sql",
        };

        // ===========================================================================
        // Invalid: PRE / POST
        // ===========================================================================
        public IEnumerable<string> InvalidPrePostSamples => new List<string>(ValidPrePostSamples) {
            @"subfolder3/Script8.sql",
            "201801021210_Script9.sql"
        };

        // ===========================================================================
        // Valid: Reference Data
        // ===========================================================================
        public int ValidRefDataSamplesRootCount => 4;
        public int ValidRefDataSamplesRecursiveCount => ValidRefDataSamples.Count();
        public string ValidRefDataSamplesLatestRoot => "99_Script99.sql";
        public string ValidRefDataSamplesLatestRecursive => "111_Script111.sql";

        public IEnumerable<string> ValidRefDataSamples => new List<string>
        {
            @"0_Script0.sql",
            @"1_Script1.sql",
            @"subfolder1/2_Script2.sql",
            $@"subfolder2/{ValidRefDataSamplesLatestRecursive}",
            $@"{ValidRefDataSamplesLatestRoot}",
            @"66_Script66.sql"
        };

        // ===========================================================================
        // Invalid: Reference Data
        // ===========================================================================
        public IEnumerable<string> InvalidRefDataSamples => new List<string>(ValidRefDataSamples) {
            @"subfolder3/Script45.sql",
            "a1_Script46.sql"
        };

    }
}
