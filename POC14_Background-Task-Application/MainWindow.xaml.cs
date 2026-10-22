using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static POC14_Background_Task_Application.Services.FileProcessorServices;


namespace POC14_Background_Task_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private FileProcessorService _service = new FileProcessorService();
        private readonly FileProcessorService service = new FileProcessorService();
        private CancellationTokenSource _cts;
        public MainWindow()
        {
            InitializeComponent();
        }


      
        private async void Button_ClickStart(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            btnCancel.IsEnabled = true;

            lstLogs.Items.Clear();

            txtStatus.Text = "Processing";

            _cancellationTokenSource = new CancellationTokenSource();

            _cts = new CancellationTokenSource();

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
                txtPercentage.Text = value + "%";

                lstLogs.Items.Add($"[{DateTime.Now:HH:mm:ss}]✔ Progress {value}%");
            });

            var operation = new Progress<string>(message =>
            {
                txtOperation.Text = message;
            });

            try
            {
                await _service.ProcessFileAsync( progress, operation,_cts.Token);

                txtStatus.Text = "Completed";

                lstLogs.Items.Add($"[{DateTime.Now:HH:mm:ss}]✔ Operation Completed");
                MessageBox.Show("Processing Completed Successfully.");

            }
            catch (OperationCanceledException)
            {
                txtStatus.Text = "Cancelled";
                txtOperation.Text = "Operation Cancelled";

                lstLogs.Items.Add($"[{DateTime.Now:HH:mm:ss}]✔ Operation Cancelled");

                MessageBox.Show("Processing Cancel Successfully.");
            }

            finally
            {
                btnStart.IsEnabled = true;
                btnCancel.IsEnabled = false;
            }

        }

        private void btnCancel_ClickCancel(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }
    }
}
