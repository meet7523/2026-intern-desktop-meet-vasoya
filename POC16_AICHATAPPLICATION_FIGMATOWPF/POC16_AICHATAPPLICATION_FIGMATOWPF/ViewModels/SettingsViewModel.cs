using POC16_AICHATAPPLICATION_FIGMATOWPF.Commands;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Helpers;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Services;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels
{
    
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly AutoLaunchService _autoLaunchService;
        private readonly AppSettingsService _settingsService;
        public ICommand CloseSettingsCommand { get; }
        public event Action CloseRequested;


        private bool _isAutoLaunchEnabled;
        private bool _isNotificationEnabled;


        private AppTheme _selectedTheme;

        public AppTheme SelectedTheme
        {
            get => _selectedTheme;

            set
            {
                if (_selectedTheme == value)
                    return;

                _selectedTheme = value;

                OnPropertyChanged();

                ApplyTheme();

                TelemetryService.Instance.TrackEvent(
               TelemetryEventName.ThemeChanged,
               TelemetryEventCategory.UserAction,
               new Dictionary<string, object>
               {
                    { "Theme", value.ToString() },
                    { "SourceTrigger", "SettingsView" },
                    { "ButtonType", "RadioButton" }
               });
            }
        }


        private AppFontSize _selectedFontSize;

        public AppFontSize SelectedFontSize
        {
            get => _selectedFontSize;

            set
            {
                if (_selectedFontSize == value)
                    return;

                _selectedFontSize = value;

                OnPropertyChanged();

                FontSizeService.ApplyFontSize(_selectedFontSize);

                SaveFontSize();
            }
        }

        public bool IsNotificationEnabled
        {
            get => _isNotificationEnabled;

            set
            {
                if (_isNotificationEnabled == value)
                    return;

                _isNotificationEnabled = value;

                OnPropertyChanged();

                Properties.Settings.Default.NotificationsEnabled = value;
                Properties.Settings.Default.Save();

                TelemetryService.Instance.TrackEvent(
                TelemetryEventName.NotificationToggled,
                TelemetryEventCategory.UserAction,
                new Dictionary<string, object>
                {
                    { "Enabled", value },
                    { "SourceTrigger", "SettingsView" },
                    { "ButtonType", "Toggle" }
                });
                }
        }

        public bool IsAutoLaunchEnabled
        {
            get => _isAutoLaunchEnabled;
            set
            {
                if (_isAutoLaunchEnabled == value)
                    return;

                _isAutoLaunchEnabled = value;

                OnPropertyChanged();

                _autoLaunchService.SetAutoLaunch(value);

                TelemetryService.Instance.TrackEvent(
                TelemetryEventName.AutoLaunchToggled,
                TelemetryEventCategory.UserAction,
                new Dictionary<string, object>
                {
                    { "Enabled", value },
                    { "SourceTrigger", "SettingsView" },
                    { "ButtonType", "Toggle" }
                });
            }
        }

        public SettingsViewModel()
        {
            _settingsService = new AppSettingsService();

            var settings = _settingsService.Load();

            _selectedTheme = settings.Theme;

            _selectedFontSize = settings.FontSize;

            FontSizeService.ApplyFontSize(_selectedFontSize);

            _autoLaunchService = new AutoLaunchService();

            _isAutoLaunchEnabled =
                _autoLaunchService.IsAutoLaunchEnabled();

            _isNotificationEnabled =
        Properties.Settings.Default.NotificationsEnabled;

            CloseSettingsCommand = new RelayCommand(CloseSettings);
        }


        private void ApplyTheme()
        {
            ThemeService.ApplyTheme(_selectedTheme);

            var settings = _settingsService.Load();

            settings.Theme = _selectedTheme;

            _settingsService.Save(settings);
        }

        private void SaveFontSize()
        {
            var settings = _settingsService.Load();

            settings.FontSize = _selectedFontSize;

            _settingsService.Save(settings);
        }

        private void CloseSettings(object parameter)
        {
            CloseRequested?.Invoke();
        }

    }
}
