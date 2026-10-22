using System;
using System.Collections.Generic;
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

namespace POC_09_SYSTEM_TRAY_APPLICATION___QUICK_NOTES
{
    /// <summary>
    /// Interaction logic for CustomBalloon.xaml
    /// </summary>
    public partial class CustomBalloon : UserControl
    {
        public CustomBalloon()
        {
            InitializeComponent();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            window.Show();

            window.WindowState = WindowState.Normal;

            window.Activate();

            window.Topmost = true;
            window.Topmost = false;

            window.Focus();
        }
    }
}
