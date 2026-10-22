using Microsoft.Win32;
using POC15__TaskManagementApp.Commands;
using POC15__TaskManagementApp.Models;
using POC15__TaskManagementApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using static POC15__TaskManagementApp.Services.LoggingService;

namespace POC15__TaskManagementApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly NavigationServices _navigationService;
        private readonly JsonService _jsonService;

        public ObservableCollection<TaskModel> Task { get; }
        public ObservableCollection<Task> Tasks { get; set; }



        public BaseViewModel CurrentView
        {
            get
            {
                return _navigationService.CurrentView;
            }
        }

        public ICommand DashboardCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand SettingsCommand { get; }
        public IEnumerable<TaskModel> FilteredTasks { get; private set; }

        public MainViewModel()
        {
            _jsonService = new JsonService();

            Task = _jsonService.LoadTasks();


            _navigationService = new NavigationServices();
            _navigationService.PropertyChanged += NavigationService_PropertyChanged;


            DashboardCommand = new RelayCommand(Dashboard);
            AddTaskCommand = new RelayCommand(AddNewTask);
            SettingsCommand = new RelayCommand(Settings);

            AddNewTask();
        }

        public void SaveAll()
        {
            _jsonService.SaveTasks(Task);
        }

        private void Dashboard()
        {
            _navigationService.Navigate(new DashboardViewModel());
        }

        private void AddNewTask()
        {
            _navigationService.Navigate(new AddTaskViewModel(_navigationService));
        }

        private void Settings()
        {
            _navigationService.Navigate(new SettingsViewModel());
        }



        private void NavigationService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_navigationService.CurrentView))
            {
                OnPropertyChanged(nameof(CurrentView));
            }
        }

    }
}
