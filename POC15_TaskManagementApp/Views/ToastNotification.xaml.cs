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
using System.Windows.Threading;

namespace POC15__TaskManagementApp.Views
{
    /// <summary>
    /// Interaction logic for ToastNotification.xaml
    /// </summary>
    public partial class ToastNotification : Window
    {
        public Action ViewTaskAction { get; set; }

        DispatcherTimer timer;

        public ToastNotification(string message)
        {
            InitializeComponent();

            txtMessage.Text = message;

            Left = SystemParameters.WorkArea.Right - Width - 10;
            Top = SystemParameters.WorkArea.Bottom - Height - 10;

            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(6);

            timer.Tick += Timer_Tick;

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
