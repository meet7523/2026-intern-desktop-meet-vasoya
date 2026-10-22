using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POC08_WPF_BASIC_FORM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnsubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtfname.Text) ||
               string.IsNullOrWhiteSpace(txtlname.Text) ||
               string.IsNullOrWhiteSpace(txtemail.Text) ||
               string.IsNullOrWhiteSpace(txtphone.Text) ||
               string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("All Fields Required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            string emailpat =
             @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            string email = txtemail.Text;

            if (!Regex.IsMatch(email, emailpat))
            {
                MessageBox.Show("enter a valid email", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Error);
                txtemail.Focus();
                return;
            }


            string phonepat = @"^[0-9]{10}$";
            string phone = txtphone.Text;

            if (!Regex.IsMatch(phone,phonepat))
            {
                MessageBox.Show("Phone number contain exactly 10 digits.", "Invalid Phone Number", MessageBoxButton.OK, MessageBoxImage.Error);
                txtphone.Focus();
                return;

            }

            MessageBox.Show("Registration Submitted Successfully!","Success",MessageBoxButton.OK,MessageBoxImage.Information);
            
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            txtfname.Clear();
            txtlname.Clear();
            txtemail.Clear();
            txtphone.Clear();
            txtAddress.Clear();
        }
    }
}