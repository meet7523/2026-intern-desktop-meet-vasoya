using POC13_NavigationApplication_MVVM.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace POC13_NavigationApplication_MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FontSettingsService SettingsService { get; }
        = new FontSettingsService();

        public static ThemeService ThemeService { get; }
             = new ThemeService();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ThemeService.ApplyTheme(
                global::POC13_NavigationApplication_MVVM.Properties.Settings.Default.Theme);
        }
    }
}
