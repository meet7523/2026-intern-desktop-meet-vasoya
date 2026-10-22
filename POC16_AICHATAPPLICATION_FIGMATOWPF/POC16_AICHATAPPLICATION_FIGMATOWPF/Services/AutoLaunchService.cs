using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public class AutoLaunchService 
    {
            private const string RegistryKeyPath =
                @"Software\Microsoft\Windows\CurrentVersion\Run";

            private const string AppName =
                "POC16_AICHATAPPLICATION_FIGMATOWPF";

            public bool IsAutoLaunchEnabled()
            {
                using (RegistryKey key =
                    Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
                {
                    if (key == null)
                        return false;

                    return key.GetValue(AppName) != null;
                }
            }

            public void SetAutoLaunch(bool enabled)
            {
                using (RegistryKey key =
                    Registry.CurrentUser.OpenSubKey(
                        RegistryKeyPath,
                        writable: true))
                {
                    if (key == null)
                        return;

                    if (enabled)
                    {
                        string exePath = Assembly.GetExecutingAssembly().Location;

                        key.SetValue(
                            AppName,
                            $"\"{exePath}\" --startup");
                    }
                    else
                    {
                        if (key.GetValue(AppName) != null)
                        {
                            key.DeleteValue(AppName);
                        }
                    }
                }
            }
        }
    }

