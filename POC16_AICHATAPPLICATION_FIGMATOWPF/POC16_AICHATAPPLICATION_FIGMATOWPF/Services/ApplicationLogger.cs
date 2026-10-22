using System;
using System.IO;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public static class ApplicationLogger
    {
        private static readonly object _lock = new object();

        private static readonly string LogDirectory =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "POC16_AICHATAPPLICATION_FIGMATOWPF",
                "Logs");

        private static readonly string LogFilePath =
            CreateNewLogFilePath();

        private static string CreateNewLogFilePath()
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);

                string timestamp =
                    DateTime.Now.ToString("yyyyMMdd_HHmmss");

                return Path.Combine(
                    LogDirectory,
                    $"ApplicationLog_{timestamp}.log");
            }
            catch
            {
                // Fallback path if log directory cannot be created.
                return Path.Combine(
                    Path.GetTempPath(),
                    $"ApplicationLog_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            }
        }

        public static void Log(string message)
        {
            try
            {
                lock (_lock)
                {
                    string timestamp =
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    string logEntry =
                        $"[{timestamp}] {message}{Environment.NewLine}";

                    File.AppendAllText(
                        LogFilePath,
                        logEntry);
                }
            }
            catch
            {
                // Logging must never crash the application.
            }
        }

        public static string GetLogFilePath()
        {
            return LogFilePath;
        }
    }
}