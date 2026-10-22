
using Hardcodet.Wpf.TaskbarNotification;
using POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon trayIcon;
 

        public QuickNotes QuickNotes { get; private set; }
        public object BalloonsIcon { get; private set; }
        public object Property { get; private set; }

        public void ShowNotification(string title, string message, BalloonIcon icon = BalloonIcon.Info)
        {

            if (!Settings.Default.EnableNotification)
                return;

            trayIcon.ShowBalloonTip(title, message, icon);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            trayIcon = (TaskbarIcon)FindResource("TrayIcon");

            ((App)Application.Current).ShowNotification("Quick Notes","Application is running in the system tray",BalloonIcon.Info);
        }

        

        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Current.MainWindow.IsVisible)
                {
                    Current.MainWindow.Hide();

                }
                else
                {
                    Current.MainWindow.Show();
                    Current.MainWindow.WindowState = WindowState.Normal;
                    Current.MainWindow.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void MenuItem_Click1(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            if(Current.MainWindow == null)
                return;

            Current.MainWindow.Show();
            Current.MainWindow.WindowState = WindowState.Normal;
            Current.MainWindow.Activate();
        }

        private void MenuItem_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (QuickNotes == null || !QuickNotes.IsVisible)
                {
                    QuickNotes = new QuickNotes();

                    QuickNotes.WindowStartupLocation = WindowStartupLocation.Manual;

                    QuickNotes.Left = SystemParameters.WorkArea.Width - QuickNotes.Width - 10;
                    QuickNotes.Top = SystemParameters.WorkArea.Height - QuickNotes.Height - 10;

                    QuickNotes.Show();
                }
                else
                {
                    QuickNotes.WindowState = WindowState.Normal;
                    QuickNotes.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MenuItem_Click6(object sender, RoutedEventArgs e)
        {
            ((TaskbarIcon)Resources["TrayIcon"]).Dispose();

            Current.Shutdown();
        }

        private void MenuItem_Click5(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsWin settings = new SettingsWin();
                settings.Show();
            }
            catch (Exception ex)
            {                        

                MessageBox.Show(ex.ToString());
            }
        }

        private void MenuItem_Click4(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }

        private void MenuItem_Click3(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }
    }
}

       

