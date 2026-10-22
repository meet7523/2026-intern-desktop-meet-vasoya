using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POC13_NavigationApplication_MVVM.Services
{
    public class StartupService : IStartupService 
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private const string AppName = "NavigationApp";

        public bool IsStartupEnabled()
        {
             RegistryKey key =
                Registry.CurrentUser.OpenSubKey(RunKey);

            return key?.GetValue(AppName) != null;
        }

        public void EnableStartup()
        {
             RegistryKey key =
                Registry.CurrentUser.OpenSubKey(RunKey, true);

            string exePath = Process.GetCurrentProcess()
                                    .MainModule
                                    .FileName;

            key?.SetValue(AppName, exePath);
        }

        public void DisableStartup()
        {
             RegistryKey key =
                Registry.CurrentUser.OpenSubKey(RunKey, true);

            key?.DeleteValue(AppName, false);
        }
    }
}

