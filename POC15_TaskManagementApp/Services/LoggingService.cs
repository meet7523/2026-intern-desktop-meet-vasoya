using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static POC15__TaskManagementApp.Services.LoggingService;

namespace POC15__TaskManagementApp.Services
{
    public static class LoggingService
    {
        public static class LoggerService
        {
            private static readonly string LogFolder =
                @"D:\TaskManagementLogs";

            private static string _logFile;

            public static void Initialize()
            {
                if (!Properties.Settings.Default.EnableLogging)
                    return;

                if (!Directory.Exists(LogFolder))
                {
                    Directory.CreateDirectory(LogFolder);
                }

                _logFile = Path.Combine(
                    LogFolder,
                    $"Log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

                Log("========================================");
                Log("Application Started");
                Log("========================================");
            }

            public static void Log(string message)
            {
                try
                {
                    File.AppendAllText(
                        _logFile,
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
                }
                catch
                {

                }
            }
        }
    }
}
        
    
 

