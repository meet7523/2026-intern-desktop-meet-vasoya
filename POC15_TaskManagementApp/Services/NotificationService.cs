using POC15__TaskManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC12__TaskManagementApp.Services
{
    public class NotificationService
    {
        public List<TaskModel> GetOverdueTasks(IEnumerable<TaskModel> tasks)
        {
            return tasks
                .Where(t => t != null &&
                            !t.IsCompleted &&
                            t.DueDate.Date < DateTime.Today)
                .ToList();
        }
    }
}
