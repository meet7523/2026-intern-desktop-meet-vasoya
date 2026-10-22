using System.Linq;
using System.Windows;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public class NavigationService : INavigationService
    {
        public void ShowMainWindow()
        {
            // Create MainWindow
            MainWindow mainWindow = new MainWindow();

            // IMPORTANT:
            // Make MainWindow the application's main window
            Application.Current.MainWindow = mainWindow;

            // Show MainWindow FIRST
            mainWindow.Show();

            // Close LoginWindow AFTER MainWindow is assigned
            Window loginWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w =>
                    w.GetType().Name == "LoginWindow");

            if (loginWindow != null)
            {
                loginWindow.Close();
            }
        }
    }
}