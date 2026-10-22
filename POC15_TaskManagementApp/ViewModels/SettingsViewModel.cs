using POC15__TaskManagementApp.Services;
using POC15__TaskManagementApp.Commands;
using POC15__TaskManagementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace POC15__TaskManagementApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public ICommand SaveCommand { get; }

        

        private string _theme;
        public string Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLightTheme));
                OnPropertyChanged(nameof(IsDarkTheme));
            }
        }

        public bool IsLightTheme
        {
            get
            {
                return Theme == "Light";
            }
            set
            {
                if (value)
                    Theme = "Light";
            }
        }

        public bool IsDarkTheme
        {
            get
            {
                return Theme == "Dark";
            }
            set
            {
                if (value)
                    Theme = "Dark";
            }
        }

        public ObservableCollection<double> FontSizes { get; set; }

        private double _selectedFontSize;
        public double SelectedFontSize
        {
            get => _selectedFontSize;
            set
            {
                if (_selectedFontSize != value)
                {
                    _selectedFontSize = value;
                    OnPropertyChanged(nameof(SelectedFontSize));
                }
            }
        }

        private bool _enableNotifications;

        public bool EnableNotifications
        {
            get => _enableNotifications;
            set
            {
                _enableNotifications = value;
                OnPropertyChanged(nameof(EnableNotifications));
            }
        }

        private bool _enableLogging;

        public bool EnableLogging
        {
            get => _enableLogging;
            set
            {
                _enableLogging = value;
                OnPropertyChanged(nameof(EnableLogging));
            }
        }





        public SettingsViewModel()
        {
            FontSizes = new ObservableCollection<double>
        {
            12,
            14,
            16,
            18,
            20,

        };
            SaveCommand = new RelayCommand(Save);

            LoadSettings();
        }

        private void LoadSettings()
        {
            Theme = Properties.Settings.Default.Theme;
            SelectedFontSize = Properties.Settings.Default.FontSize;
            EnableNotifications = Properties.Settings.Default.EnableNotification;
            EnableLogging = Properties.Settings.Default.EnableLogging;
        }

        private void Save()
        {
            Properties.Settings.Default.Theme = Theme;

            Properties.Settings.Default.FontSize = SelectedFontSize;

            Properties.Settings.Default.EnableNotification = EnableNotifications;

            Properties.Settings.Default.EnableLogging = EnableLogging;

            Properties.Settings.Default.Save();

            ThemeService.ApplyTheme(Theme);
            Application.Current.Resources["AppFontSize"] = SelectedFontSize;

            MessageBox.Show("Settings saved successfully.",
                            "Settings",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }


    }
}
