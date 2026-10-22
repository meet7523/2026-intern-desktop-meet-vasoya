using POC15__TaskManagementApp.Services;
using POC15__TaskManagementApp.Commands;
using POC15__TaskManagementApp.Models;
using POC15__TaskManagementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static POC15__TaskManagementApp.Services.LoggingService;

namespace POC15__TaskManagementApp.ViewModels
{
    public class EditTaskViewModel : BaseViewModel
    {
        private readonly NavigationServices _navigationService;
        private readonly JsonService _jsonService;
        private readonly TaskModel _task;
        public ObservableCollection<TaskModel> Tasks { get; set; }

        private DateTime? _dueDate;

        public CalendarViewModel Calendar { get; }

        public ICommand UpdateTaskCommand { get; }
        public ICommand CancelCommand { get; }

        




        public EditTaskViewModel(NavigationServices navigationService, TaskModel task)
        {
            _navigationService = navigationService;
            _jsonService = new JsonService();

            _task = task;

             MinimumDueDate = DateTime.Today;

    Calendar = new CalendarViewModel();

    Calendar.DateSelected += Calendar_DateSelected;

    Title = task.Title;
    Description = task.Description;

    // Load existing Due Date
    _dueDate = task.DueDate;

    // Show existing Due Date in Calendar text
    Calendar.SelectedDate = task.DueDate;

    Calendar.DisplayedMonth = task.DueDate;


            Priority = task.Priority;
            IsCompleted = task.IsCompleted;

            UpdateTaskCommand = new RelayCommand(UpdateTask);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Calendar_DateSelected(
          object sender,
          EventArgs e)
        {
            DueDate = Calendar.SelectedDate;
        }


        public ObservableCollection<string> Priorities { get; } =
       new ObservableCollection<string>
       {
            "Priority",
            "High",
            "Medium",
            "Low"
       };



        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }


        public DateTime MinimumDueDate { get; }

        public DateTime? DueDate
        {
            get => _dueDate;
            set
            {

                if (value.HasValue && value.Value.Date < MinimumDueDate.Date)
                {
                    value = MinimumDueDate;
                }

                if (_dueDate != value)
                {
                    _dueDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _priority = "Priority";
        public string Priority
        {
            get => _priority;
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




        public ObservableCollection<TaskModel> OverdueTaskLists { get; set; }
            = new ObservableCollection<TaskModel>();

        public object TotalTasks { get; private set; }
        public int CompletedTasks { get; private set; }
        public int IncompleteTasks { get; private set; }
        public int OverdueTasks { get; private set; }

        private void UpdateTask()
        {
            var tasks = _jsonService.LoadTasks();

            UpdateDashboard();

            var existingTask = tasks.FirstOrDefault(t => t.Id == _task.Id);

            if (existingTask != null)
            {
                existingTask.Title = Title;
                existingTask.Description = Description;
                existingTask.DueDate = DueDate ?? DateTime.Today;
                existingTask.Priority = Priority;
                existingTask.IsCompleted = IsCompleted;
                existingTask.CompletedDate = IsCompleted
                    ? DateTime.Now
                    : (DateTime?)null;

                _jsonService.SaveTasks(tasks);
                LoggerService.Log("Task Updated");

                MessageBox.Show("Task updated successfully.");

                _navigationService.Navigate(
                    new TaskDetailViewModel(_navigationService));
            }
        }

        private void UpdateDashboard()
        {
            if (Tasks == null)
                return;

            TotalTasks = Tasks.Count;

            CompletedTasks = Tasks.Count(t => t.IsCompleted);

            IncompleteTasks = Tasks.Count(t => !t.IsCompleted);

            OverdueTasks = Tasks.Count(t =>
                t.DueDate.Date < DateTime.Today &&
                !t.IsCompleted);
        }

        private void Cancel()
        {
            _navigationService.Navigate(
                new TaskDetailViewModel(_navigationService));
        }

    }
}
