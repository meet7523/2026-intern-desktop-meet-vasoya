using POC12__TaskManagementApp.Services;
using POC13__TaskManagementApp.Services;
using POC15__TaskManagementApp.Models;
using POC15__TaskManagementApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POC15__TaskManagementApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public ObservableCollection<TaskModel> Tasks { get; set; }
        private readonly JsonService _jsonService;
        private readonly ToastService _toastService;

        public int TotalTasks => Tasks.Count;

        public int CompletedTasks =>
            Tasks.Count(t => t.IsCompleted);

        public int IncompleteTasks =>
            Tasks.Count(t => !t.IsCompleted);

        public int OverdueTasks =>
            Tasks.Count(t => t.DueDate.Date < DateTime.Today &&
                             !t.IsCompleted);


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


        


        public DashboardViewModel()
        {
            _jsonService = new JsonService();
            _toastService = new ToastService();

            Tasks = _jsonService.LoadTasks();

            UpdateDashboard();

            if (Properties.Settings.Default.EnableNotification)
            {
                ShowDueDateNotification();
            }


        }


        private void UpdateDashboard()
        {
            OnPropertyChanged(nameof(TotalTasks));
            OnPropertyChanged(nameof(CompletedTasks));
            OnPropertyChanged(nameof(IncompleteTasks));
            OnPropertyChanged(nameof(OverdueTasks));
        }

        private void ShowDueDateNotification()
        {
            var overdue = Tasks
                .Where(t => !t.IsCompleted &&
                            t.DueDate.Date < DateTime.Today)
                .ToList();

            if (!overdue.Any())
                return;

            string msg = $"You have {overdue.Count} overdue task(s)\n\n";

            foreach (var task in overdue)
            {
                msg += $"• {task.Title}\n";
            }

            _toastService.Show(msg);
        }


    }
}