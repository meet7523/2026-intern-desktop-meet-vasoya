using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using System;
using System.Windows;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public static class ThemeService
    {
        public static void ApplyTheme(AppTheme theme)
        {
            var app = Application.Current;

            if (app == null)
                return;

            var dictionaries = app.Resources.MergedDictionaries;

            dictionaries.Clear();

            ResourceDictionary themeDictionary =
                new ResourceDictionary();

            if (theme == AppTheme.Dark)
            {
                themeDictionary.Source = new Uri(
                    "Themes/DarkTheme.xaml",
                    UriKind.Relative);
            }
            else
            {
                themeDictionary.Source = new Uri(
                    "Themes/LightTheme.xaml",
                    UriKind.Relative);
            }

            dictionaries.Add(themeDictionary);
        }
    }
}