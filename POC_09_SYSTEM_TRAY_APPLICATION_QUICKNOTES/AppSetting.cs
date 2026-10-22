using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES
{
    internal class AppSetting
    {
        public static bool EnableNotification
        {
            get
            {
                return Properties.Settings.Default.EnableNotification;
            }
            set
            {
                Properties.Settings.Default.EnableNotification = value;
                Properties.Settings.Default.Save();
            }
        }

        public static object Default { get; internal set; }
    }
}
