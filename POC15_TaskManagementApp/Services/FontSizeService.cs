using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POC15__TaskManagementApp.Services
{
    public static class FontSizeService
    {
        public static void ApplyFontSize(double fontSize)
        {
            Application.Current.Resources["AppFontSize"] = fontSize;
        }
    }
}

