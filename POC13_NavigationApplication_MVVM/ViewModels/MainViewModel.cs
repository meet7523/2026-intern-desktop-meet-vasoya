using POC13_NavigationApplication_MVVM.Commands;
using POC13_NavigationApplication_MVVM.Models;
using POC13_NavigationApplication_MVVM.Services;
using POC13_NavigationApplication_MVVM.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace POC13_NavigationApplication_MVVM.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService navigationService;



        public ObservableCollection<NavigationItem> MenuItems { get; }

        public ICommand HomeCommand { get; }
        public ICommand ProductsCommand { get; }
        public ICommand SettingsCommand { get; }


        private BaseViewModel _currentViewModel;
        private string _selectedMenu;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            navigationService = new Services.NavigationService(this);

            HomeCommand = new RelayCommand(() => NavigateToHome());
            ProductsCommand = new RelayCommand(() => NavigateToProducts());
            SettingsCommand = new RelayCommand(() => NavigateToSettings());

            string page = Properties.Settings.Default.StartupPage;

            switch (page)
            {
                case "Products":
                    NavigateToProducts();
                    break;

                case "Settings":
                    NavigateToSettings();
                    break;

                default:
                    NavigateToHome();
                    break;
            }


            //navigationService = new Services.NavigationService(this);

            //HomeCommand = new RelayCommand(() =>
            //    navigationService.NavigateTo(new HomeViewModel()));

            //ProductsCommand = new RelayCommand(() =>
            //    navigationService.NavigateTo(new ProductsViewModel()));

            //SettingsCommand = new RelayCommand(() =>
            //    navigationService.NavigateTo(new SettingsViewModel()));

            //CurrentViewModel = new HomeViewModel();
        }


        private void NavigateToHome()
        {
            navigationService.NavigateTo(new HomeViewModel());
        }

        private void NavigateToProducts()
        {
            navigationService.NavigateTo(new ProductsViewModel());
        }

        private void NavigateToSettings()
        {
            navigationService.NavigateTo(new SettingsViewModel());
        }
    }
}

        


    

