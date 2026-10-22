using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
namespace POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES
{
    /// <summary>
    /// Interaction logic for SettingsWin.xaml
    /// </summary>
    public partial class SettingsWin : Window
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private const string AppName = "Quick Notes";
        private object chkEnableNotification;
        private object notifyIcon;

        public object AppSettings { get; private set; }

        public SettingsWin()
        {
            InitializeComponent();
           

            LoadStartupSetting();
        }

        


        private void AddApplicationToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, true))
            {
                if (key == null)
                    return;

                string exePath = System.Diagnostics.Process
                                .GetCurrentProcess()
                                .MainModule
                                .FileName;

                key.SetValue(AppName, $"\"{"D:\\2026-intern-desktop-meet-vasoya\\POC08_WPF BASIC FORM\\POC_09 SYSTEM TRAY APPLICATION – QUICK NOTES\\bin\\Debug\\POC_09 SYSTEM TRAY APPLICATION – QUICK NOTES.exe"}\"");
            }
        }

        private void RemoveApplicationFromStartup()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, true);

            if (key == null)
                return;

            key.DeleteValue(AppName, false);
        }

        private void LoadStartupSetting()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey);

            if (key == null)
            {
                chkStartWithWindows.IsChecked = false;
                return;
            }

            chkStartWithWindows.IsChecked = key.GetValue(AppName) != null;
            chkEnableNotifications.IsChecked = Properties.Settings.Default.EnableNotification;

            double opacity = Properties.Settings.Default.WindowOpacity;

            if (opacity < 0.30 || opacity > 1.00)
            {
                opacity = 1.00;
            }

            sliderOpacity.Value = opacity * 100;

            txtOpacityValue.Text = $"{sliderOpacity.Value:0}%";


            double fontSize = Properties.Settings.Default.NotesFontSize;

            if (fontSize < 12 || fontSize > 20)
                fontSize = 14;

            foreach (ComboBoxItem item in cmbFontSize.Items)
            {
                if (item.Content.ToString() == fontSize.ToString())
                {
                    cmbFontSize.SelectedItem = item;
                    break;
                }
            }


            string theme = Properties.Settings.Default.AppTheme;

            foreach (ComboBoxItem item in cmbTheme.Items)
            {
                if (item.Content.ToString() == theme)
                {
                    cmbTheme.SelectedItem = item;
                    break;
                }
            }
        }

        private void chkStartWithWindows_Checked(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, true);

            if (key == null)
                return;

            string appPath = System.Reflection.Assembly
                .GetExecutingAssembly()
                .Location;

            string AppPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            key.SetValue(AppName, $"\"{"D:\\2026-intern-desktop-meet-vasoya\\POC08_WPF BASIC FORM\\POC_09 SYSTEM TRAY APPLICATION – QUICK NOTES\\bin\\Debug\\POC_09 SYSTEM TRAY APPLICATION – QUICK NOTES.exe"}\"");
        }

        private void chkStartWithWindows_Unchecked(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, true);

            if (key == null)
                return;

            key.DeleteValue(AppName, false);
        }

        private void Button_ClickSave(object sender, RoutedEventArgs e)
        {

            try
            {

                Properties.Settings.Default.StartWithWindows =chkStartWithWindows.IsChecked == true;
                Properties.Settings.Default.EnableNotification = chkEnableNotifications.IsChecked == true;

                double opacity = sliderOpacity.Value / 100.0;

                opacity = Math.Max(0.30, Math.Min(opacity, 1.00));


                Properties.Settings.Default.WindowOpacity = opacity;


                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.Opacity = opacity;
                }

                if (cmbFontSize.SelectedItem is ComboBoxItem item)
                {
                    double fontSize = Convert.ToDouble(item.Content);

                    Properties.Settings.Default.NotesFontSize = fontSize;
                    Properties.Settings.Default.Save();

                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.ApplyFontSize(fontSize);
                    }
                }


                if (cmbTheme.SelectedItem is ComboBoxItem themeItem)
                {
                    string theme = themeItem.Content.ToString();

                    Properties.Settings.Default.AppTheme = theme;

                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.ApplyTheme(theme);
                    }
                }







                Properties.Settings.Default.Save();

                if (chkStartWithWindows.IsChecked == true)
                {
                    AddApplicationToStartup();
                }
                else
                {
                    RemoveApplicationFromStartup();
                }

                MessageBox.Show("Settings saved successfully.", "Quick Notes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_ClickCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

       

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtOpacityValue.Text = $"{sliderOpacity.Value:0}%";
        }

       
    }
}
            
            
      

    
  


           



        
    

