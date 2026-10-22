using Newtonsoft.Json;
using POC15__TaskManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static POC15__TaskManagementApp.Services.LoggingService;

namespace POC15__TaskManagementApp.Services
{
    public class JsonService
    {
        private readonly string filePath = "D:\\meet\\tasks.json";
        private object tasks;

        public ObservableCollection<TaskModel> LoadTasks()
        {
            if (!File.Exists(filePath))
                return new ObservableCollection<TaskModel>();
        

            string json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<ObservableCollection<TaskModel>>(json) ?? new ObservableCollection<TaskModel>();
            
        }
        public void SaveTasks(ObservableCollection<TaskModel> tasks)
        {
            string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText(filePath, json);

        }

    }
}
