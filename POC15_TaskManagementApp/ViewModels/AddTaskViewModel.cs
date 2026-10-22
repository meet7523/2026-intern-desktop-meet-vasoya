using POC15__TaskManagementApp.Services;
using POC15__TaskManagementApp.Commands;
using POC15__TaskManagementApp.Models;
using POC15__TaskManagementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using static POC15__TaskManagementApp.Services.LoggingService;

namespace POC15__TaskManagementApp.ViewModels
{
    public class AddTaskViewModel : BaseViewModel
    {
        private readonly JsonService _jsonService;
        private readonly NavigationServices _navigationService;

        public ObservableCollection<TaskModel> Tasks { get; set; }

        private DateTime? _dueDate;
        public DateTime? SelectedDate { get; set; }

        public CalendarViewModel Calendar { get; }


        public ICommand SaveTaskCommand { get; }
        public ICommand ViewTaskCommand { get; }


       





        public AddTaskViewModel(NavigationServices navigationService)
        {
            _navigationService = navigationService;

            _jsonService = new JsonService();

            Tasks = _jsonService.LoadTasks();
            UpdateDashboard();

            Calendar = new CalendarViewModel();

            Calendar.DateSelected +=
                Calendar_DateSelected;

            SaveTaskCommand = new RelayCommand(SaveTask);
            ViewTaskCommand = new RelayCommand(ViewTask);

         
        }

        private void Calendar_DateSelected(
           object sender,
           EventArgs e)
        {
            DueDate = Calendar.SelectedDate;
        }


        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }


        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }


        public DateTime? DueDate
        {
            get => _dueDate;
            set
            {
                if (_dueDate != value)
                {
                    _dueDate = value;
                    OnPropertyChanged();
                }
            }
        }





        private object txttitle;

        public ObservableCollection<string> Priorities { get; } =
        new ObservableCollection<string>
        {
            "Priority",
            "High",
            "Medium",
            "Low"
        };

        private string _priority = "Priority";

        public string Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                OnPropertyChanged("Priority");
            }
        }

        private bool _isCompleted;

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }



        private string _titleError;
        public string TitleError
        {
            get => _titleError;
            set
            {
                _titleError = value;
                OnPropertyChanged();
            }
        }

        private string _descriptionError;
        public string DescriptionError
        {
            get => _descriptionError;
            set
            {
                _descriptionError = value;
                OnPropertyChanged();
            }
        }



        private string _dueDateError;
        public string DueDateError
        {
            get => _dueDateError;
            set
            {
                _dueDateError = value;
                OnPropertyChanged();
            }
        }


        private string _priorityError;
        public string PriorityError
        {
            get => _priorityError;
            set
            {
                _priorityError = value;
                OnPropertyChanged();
            }
        }


        private string _notificationMessage;

        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                _notificationMessage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TaskModel> OverdueTaskLists { get; set; }
            = new ObservableCollection<TaskModel>();

        public int TotalTasks { get; private set; }
        public int CompletedTasks { get; private set; }
        public int IncompleteTasks { get; private set; }
        public int OverdueTasks { get; private set; }
        public object OverdueTaskList { get; private set; }
        public object NewTask { get; private set; }
        public string SelectedTask { get; private set; }

        private void SaveTask()
        {
            TitleError = "";
            DescriptionError = "";
            DueDateError = "";
            PriorityError = "";


            bool valid = true;

            if (string.IsNullOrWhiteSpace(Title))
            {
                TitleError = "Task Title is required.";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                DescriptionError = "Description is required.";
                valid = false;
            }

            if (DueDate < DateTime.Today)
            {
                DueDateError = "Due Date cannot be earlier than today.";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(Priority))
            {
                PriorityError = "Please select Priority.";
                valid = false;
            }

            if (!valid)
                return;



            TaskModel task = new TaskModel
            {
                Id = Guid.NewGuid(),
                Title = Title.Trim(),
                Description = Description.Trim(),
                DueDate = DueDate ?? DateTime.Today,    
                Priority = Priority,
                IsCompleted = IsCompleted,
                CreatedDate = DateTime.Now,
                CompletedDate = IsCompleted ? DateTime.Now : (DateTime?)null
            };

            Tasks.Add(task);

            _jsonService.SaveTasks(Tasks);
            LoggerService.Log("Tasks Added");

            MessageBox.Show("Task Saved Successfully.");

            


            ClearTask();
        }




        private void ViewTask()
        {
            _navigationService.Navigate(new TaskDetailViewModel(_navigationService));
        }

        private void UpdateDashboard()
        {
            TotalTasks = Tasks.Count;

            CompletedTasks = Tasks.Count(t => t.IsCompleted);

            IncompleteTasks = Tasks.Count(t => !t.IsCompleted);

            OverdueTasks = Tasks.Count(t =>
                t.DueDate.Date < DateTime.Today &&
                !t.IsCompleted);
        }

        private void ClearTask()
        {
            Title = "";
            Description = "";
            Priority = "Priority";
            DueDate = null;
            IsCompleted = false;
            SelectedDate = null;
            Calendar.SelectedDate = null;
        }

        
    }
}