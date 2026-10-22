using POC13_NavigationApplication_MVVM.Commands;
using POC13_NavigationApplication_MVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POC13_NavigationApplication_MVVM.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
     
            private readonly IStartupService _startupService;
            private readonly ThemeService _themeService;
            public event Action CloseRequested;

        private bool _startWithWindows;
            public bool StartWithWindows
            {
                get => _startWithWindows;
                set => SetProperty(ref _startWithWindows, value);
            }

            public ObservableCollection<double> FontSizes { get; }

            private double _selectedFontSize;
            public double SelectedFontSize
            {
                get => _selectedFontSize;
                set => SetProperty(ref _selectedFontSize, value);
            }


        public ObservableCollection<string> Themes { get; }

        private string _selectedTheme;

        public string SelectedTheme
        {
            get => _selectedTheme;
            set => SetProperty(ref _selectedTheme, value);
        }


        public ObservableCollection<string> StartPages { get; }

        private string _selectedStartPage;
        public string SelectedStartPage
        {
            get => _selectedStartPage;
            set => SetProperty(ref _selectedStartPage, value);
        }


            public ICommand SaveCommand { get; }
            public ICommand CancelCommand { get; }

            public SettingsViewModel()
                : this(new StartupService())
            {
           
            }

            public SettingsViewModel(IStartupService startupService)
            {
                CancelCommand = new RelayCommand(Cancel);

                _startupService = startupService;
                
                StartWithWindows = Properties.Settings.Default.StartWithWindows;



            FontSizes = new ObservableCollection<double>
                {
                    12,
                    14,
                    16,
                    18,
                    20
                };

            SelectedFontSize = Properties.Settings.Default.FontSize;


            Themes = new ObservableCollection<string>
            {
                "Light",
                "Dark"
            };

            SelectedTheme = Properties.Settings.Default.Theme;


            StartPages = new ObservableCollection<string>
            {
                "Home",
                "Products",
                "Settings"
            };

            SelectedStartPage = Properties.Settings.Default.StartupPage;


            SaveCommand = new RelayCommand(Save);
                CancelCommand = new RelayCommand(Cancel);
            }

        private void Save()
        {
            try
            {

                Properties.Settings.Default.StartWithWindows = StartWithWindows;
                Properties.Settings.Default.Save();

                if (StartWithWindows)
                {
                    _startupService.EnableStartup();
                }
                else
                {
                    _startupService.DisableStartup();
                }


                Properties.Settings.Default.FontSize = SelectedFontSize;
                Properties.Settings.Default.Save();

                App.SettingsService.FontSize = SelectedFontSize;


                Properties.Settings.Default.Theme = SelectedTheme;
                Properties.Settings.Default.Save();


                Properties.Settings.Default.StartupPage = SelectedStartPage ?? "Home";
                Properties.Settings.Default.Save();





                App.SettingsService.FontSize = SelectedFontSize;

                MessageBox.Show(
                    "Settings saved successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            
        }

        
    }
}

