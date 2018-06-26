using Microsoft.Build.Utilities;
using System.Text;

namespace CustomSSDTMigrationScripts
{
    public static class Logger
    {
        public static TaskLoggingHelper logger;
        private static StringBuilder LogBuffer { get; set; } = new StringBuilder();

        public static void LogMessage(string msg)
        {
            LogBuffer.AppendLine($"Information: {msg}");
            try { logger.LogMessage(msg); } catch { }
        }

        public static void LogWarning(string msg)
        {
            LogBuffer.AppendLine($"Warning: {msg}");
            try { logger.LogWarning(msg); } catch { }
        }

        public static void LogError(string msg)
        {
            LogBuffer.AppendLine($"Error: {msg}");
            try { logger.LogError(msg); } catch { }
        }
    }
}
