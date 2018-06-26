using System.IO;

namespace CustomSSDTMigrationScripts
{
    internal static class TSqlHelper
    {
        internal static string GetScopedTsqlByFile(string filePath)
        {
            var content = File.ReadAllText(filePath).Replace("'", "''");

            return $"exec('{content}')";
        }
    }
}
