using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC15__TaskManagementApp.Models
{
    
    public class TaskModel
    {
        public string ShortId => Id.ToString().Substring(0, 8);
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }

        public int PriorityOrder
        {
            get
            {
                switch (Priority)
                {
                    case "High":
                        return 1;

                    case "Medium":
                        return 2;

                    case "Low":
                        return 3;

                    default:
                        return 4;
                }
            }
        }

        public bool IsOverdue
        {
            get
            {
                return DueDate.Date < DateTime.Today && !IsCompleted;
            }
        }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
