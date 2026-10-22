using Microsoft.Win32;
using POC12__TaskManagementApp.Services;
using POC15__TaskManagementApp.Commands;
using POC15__TaskManagementApp.Models;
using POC15__TaskManagementApp.Services;
using POC15__TaskManagementApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using System.IO;
using static POC15__TaskManagementApp.Services.LoggingService;

namespace POC15__TaskManagementApp.ViewModels
{
    public class TaskDetailViewModel : BaseViewModel
    {
        private readonly JsonService _jsonService;
        private readonly NavigationServices _navigationService;
        public ObservableCollection<TaskModel> Tasks { get; set; }
        public ICollectionView FilteredTasks { get; set; }


        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public ICommand ExportTasksCommand { get; }
        public ICommand BackCommand { get; }

        public ICommand ResetFilterCommand { get; }

        private TaskModel _selectedTask;

        public TaskModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
            }
        }


        private string _selectedStatus = "All Tasks";

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();

                FilteredTasks.Refresh();
                UpdateNoDataMessage();
                LoggerService.Log("Status Filter Applied");
            }
        }


        private string _selectedPriority = "All Priority";

        public string SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                if (_selectedPriority != value)
                {
                    _selectedPriority = value;
                    OnPropertyChanged();
                    FilteredTasks.Refresh();
                    UpdateNoDataMessage();
                    LoggerService.Log("Priority Filter Applied");
                }
            }
        }


        private string _selectedDueDate = "All Dates";

        public string SelectedDueDate
        {
            get => _selectedDueDate;
            set
            {
                if (_selectedDueDate != value)
                {
                    _selectedDueDate = value;
                    OnPropertyChanged();

                    FilteredTasks?.Refresh();
                    UpdateNoDataMessage();

                    LoggerService.Log("Due Date Filter Applied");
                }
            }
        }

        private string SelectedDueDates;
        private string _noDataMessage;

        public string NoDataMessage
        {
            get => _noDataMessage;
            set
            {
                _noDataMessage = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();

                    FilteredTasks.Refresh();
                    UpdateNoDataMessage();
                }
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


        private string _selectedSortOption = "All Categories";

        public string SelectedSortOption
        {
            get { return _selectedSortOption; }
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged("SelectedSortOption");

                ApplySorting();
            }
        }





        public ObservableCollection<string> StatusFilters { get; } =
    new ObservableCollection<string>
        {
            "All Tasks",
            "Active",
            "Completed",
        };

        public ObservableCollection<string> PriorityFilters { get; } =
            new ObservableCollection<string>
            {
                "All Priority",
                "High",
                "Medium",
                "Low"
            };

        public ObservableCollection<string> DueDateFilters { get; } =
            new ObservableCollection<string>
            {
                "All Dates",
                "Today",
                "This Week",
                "Overdue"
            };

        public ObservableCollection<string> SortOption { get; } =
             new ObservableCollection<string>
         {
            "All Categories",
            "Title Ascending",
            "Title Descending",

            "Due Date Earliest",
            "Due Date Latest",

            "Priority High-Low",
            "Priority Low-High"
        };

        public int TotalTasks { get; private set; }
        public int CompletedTasks { get; private set; }
        public int IncompleteTasks { get; private set; }
        public int OverdueTasks { get; private set; }

        public TaskDetailViewModel(NavigationServices navigationService)
        {
            _jsonService = new JsonService();
            _navigationService = navigationService;

            SelectedSortOption = "All Categories";

            Tasks = _jsonService.LoadTasks();
            UpdateDashboard();
            LoggerService.Log("Task List Refreshed");

            FilteredTasks = CollectionViewSource.GetDefaultView(Tasks);
            FilteredTasks.Filter = FilterTask;


            EditTaskCommand = new RelayCommand(EditTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            ExportTasksCommand = new RelayCommand(ExportTasks);
            BackCommand = new RelayCommand(Back);
            ResetFilterCommand = new RelayCommand(ResetFilters);
        }


        private void LoadTasks()
        {
            Tasks = _jsonService.LoadTasks();

        }


        private void ResetFilters()
        {
            SelectedStatus = "All Tasks";
            SelectedPriority = "All Priority";
            SelectedDueDate = "All Dates";
            SelectedSortOption = "All Categories";
            SearchText = string.Empty;
            FilteredTasks.Refresh();
            LoggerService.Log("Refresh Button Clicked");

        }



        private void EditTask()
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Please select a task to edit.",
                                "Edit Task",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            _navigationService.Navigate(new EditTaskViewModel(_navigationService, SelectedTask));
        }

        private void DeleteTask()
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Please select a task to delete.",
                                "Delete Task",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Delete '{SelectedTask.Title}' ?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Tasks.Remove(SelectedTask);

                _jsonService.SaveTasks(Tasks);

                LoggerService.Log("Task Deleted");
                MessageBox.Show("Task deleted successfully.",
                                "Delete",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private void Back()
        {
            _navigationService.Navigate(
                new AddTaskViewModel(_navigationService));
        }

        private bool FilterTask(object obj)
        {
            if (!(obj is TaskModel task))
                return false;


            bool statusMatch = true;

            switch (SelectedStatus)
            {

                case "Active":
                    statusMatch = !task.IsCompleted;
                    break;

                case "Completed":
                    statusMatch = task.IsCompleted;
                    break;


                default:
                    statusMatch = true;
                    break;

            }



            bool priorityMatch = true;

            switch (SelectedPriority)
            {
                case "High":
                    priorityMatch = task.Priority == "High";
                    break;

                case "Medium":
                    priorityMatch = task.Priority == "Medium";
                    break;

                case "Low":
                    priorityMatch = task.Priority == "Low";
                    break;

                    default:
                    priorityMatch = true;
                    break;
            }

           
       
            bool dueDateMatch = true;

            DateTime today = DateTime.Today;

            switch (SelectedDueDate)
            {
                case "Today":
                    dueDateMatch = task.DueDate.Date == today;
                    break;

                case "This Week":
                    DateTime endOfWeek = today.AddDays(7);
                    dueDateMatch = task.DueDate.Date >= today &&
                                   task.DueDate.Date <= endOfWeek;
                    break;

                case "Overdue":
                    dueDateMatch = task.DueDate.Date < today &&
                                   !task.IsCompleted;
                    break;

                default:
                    dueDateMatch = true;
                    break;
            }

            bool searchMatch = true;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.ToLower();

                searchMatch =
                    (!string.IsNullOrEmpty(task.Title) &&
                     task.Title.ToLower().Contains(search))
                    ||
                    (!string.IsNullOrEmpty(task.Description) &&
                     task.Description.ToLower().Contains(search));
            }

            return statusMatch &&
                   priorityMatch &&
                   dueDateMatch &&
                   searchMatch;


        }

        private void UpdateNoDataMessage()
        {
            if (FilteredTasks.IsEmpty)
            {
                if (SelectedStatus != "All Tasks")
                {
                    NoDataMessage = $"{SelectedStatus} tasks not found.";
                }
                else if (SelectedPriority != "All Priority")
                {
                    NoDataMessage = $"{SelectedPriority} priority tasks not found.";
                }
                else
                {
                    NoDataMessage = "No tasks found.";
                }
            }
            else
            {
                NoDataMessage = string.Empty;
            }
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

        private void ApplySorting()
        {
            if (FilteredTasks == null)
                return;

            FilteredTasks.SortDescriptions.Clear();

            switch (SelectedSortOption)
            {
                case "Title Ascending":
                    FilteredTasks.SortDescriptions.Add(
                        new SortDescription("Title", ListSortDirection.Ascending));
                    break;

                case "Title Descending":
                    FilteredTasks.SortDescriptions.Add(
                        new SortDescription("Title", ListSortDirection.Descending));
                    break;

                case "Due Date Earliest":
                    FilteredTasks.SortDescriptions.Add(
                        new SortDescription("DueDate", ListSortDirection.Ascending));
                    break;

                case "Due Date Latest":
                    FilteredTasks.SortDescriptions.Add(
                        new SortDescription("DueDate", ListSortDirection.Descending));
                    break;

                case "Priority High-Low":
                    FilteredTasks.SortDescriptions.Add(
                        new SortDescription("PriorityOrder", ListSortDirection.Ascending));
                    break;

                case "Priority Low-High":
                    FilteredTasks.SortDescriptions.Add(
                        new SortDescription("PriorityOrder", ListSortDirection.Descending));
                    break;

            }

            FilteredTasks.Refresh();
        }


        private void ExportToText(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("===============================================");
            sb.AppendLine("           TASK MANAGEMENT REPORT");
            sb.AppendLine("===============================================");
            sb.AppendLine("Generated On : " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
            sb.AppendLine();

            foreach (TaskModel task in FilteredTasks)
            {
                sb.AppendLine("-----------------------------------------------");
                sb.AppendLine("Task ID      : " + task.ShortId);
                sb.AppendLine("Title        : " + task.Title);
                sb.AppendLine("Description  : " + task.Description);
                sb.AppendLine("Due Date     : " + task.DueDate.ToString("dd-MMM-yyyy"));
                sb.AppendLine("Priority     : " + task.Priority);
                sb.AppendLine("Status       : " + (task.IsCompleted ? "Completed" : "Pending"));

                if (task.IsCompleted)
                {
                    sb.AppendLine("Completed On : " +
                        (task.CompletedDate.HasValue
                            ? task.CompletedDate.Value.ToString("dd-MMM-yyyy HH:mm")
                            : "-"));
                }

                sb.AppendLine("-----------------------------------------------");
                sb.AppendLine();
            }

            sb.AppendLine("===============================================");
            sb.AppendLine("Total Tasks : " + FilteredTasks.Cast<TaskModel>().Count());
            sb.AppendLine("===============================================");

            File.WriteAllText(filePath, sb.ToString());
        }


        private void ExportTasks()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Text File (*.txt)|*.txt";
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.FileName = "TaskReport";

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportToText(saveFileDialog.FileName);

                LoggerService.Log("Tasks Exported");

                MessageBox.Show("Tasks exported successfully!",
                                "Export",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

    }
}



    
        
    

