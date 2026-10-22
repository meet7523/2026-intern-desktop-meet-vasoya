using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POC15__TaskManagementApp.Services
{
    public class ThemeService
    {
        public static void ApplyTheme(string theme)
        {
            ResourceDictionary dictionary = new ResourceDictionary();

            dictionary.Source =
                new Uri(
                theme == "Dark"
                ? "Themes/DarkTheme.xaml"
                : "Themes/LightTheme.xaml",
                UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Clear();

            Application.Current.Resources.MergedDictionaries.Add(dictionary);
        }
    }
}
