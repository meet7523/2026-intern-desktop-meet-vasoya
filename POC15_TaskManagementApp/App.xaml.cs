using POC12__TaskManagementApp.Services;
using POC15__TaskManagementApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static POC15__TaskManagementApp.Services.LoggingService;

namespace POC15__TaskManagementApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LoggerService.Initialize();
            LoggerService.Log("Task Loaded");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LoggerService.Log("Application Closed");

            base.OnExit(e);
        }
    }
}
