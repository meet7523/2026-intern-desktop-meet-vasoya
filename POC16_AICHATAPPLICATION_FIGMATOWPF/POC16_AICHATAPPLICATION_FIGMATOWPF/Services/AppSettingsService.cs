using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using System;
using System.IO;
using System.Text.Json;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public class AppSettings
    {
        public AppTheme Theme { get; set; } = AppTheme.Light;

        public AppFontSize FontSize { get; set; } = AppFontSize.Medium;
    }

    public class AppSettingsService
    {
        private readonly string _settingsDirectory;
        private readonly string _settingsFile;

        public AppSettingsService()
        {
            _settingsDirectory = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                "POC16_AICHATAPPLICATION_FIGMATOWPF");

            _settingsFile = Path.Combine(
                _settingsDirectory,
                "settings.json");
        }

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_settingsFile))
                {
                    return new AppSettings();
                }

                string json = File.ReadAllText(_settingsFile);

                var settings = JsonSerializer.Deserialize<AppSettings>(json);

                return settings ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            try
            {
                if (!Directory.Exists(_settingsDirectory))
                {
                    Directory.CreateDirectory(_settingsDirectory);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(
                    settings,
                    options);

                File.WriteAllText(_settingsFile, json);
            }
            catch
            {
                // You can add logging here later.
            }
        }
    }
}