using POC12_LoginApplicationWith_MVVM.Commands;
using POC12_LoginApplicationWith_MVVM.Services;
using POC12_LoginApplicationWith_MVVM.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static POC12_LoginApplicationWith_MVVM.Services.AuthenticationService;

namespace POC12_LoginApplicationWith_MVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthenticationService _authenticationService;

        public RelayCommand LoginCommand { get; }
        public RelayCommand CancelCommand { get; }
        public Action Login { get; }
        public Action Cancel { get; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordBox.Clear();
            this.Close();
        }

        private void txtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordBox.Password != txtPassword.Text)
            {
                PasswordBox.Password = txtPassword.Text;
            }

            if (DataContext is MainViewModel vm)
            {
                vm.Password = txtPassword.Text;
            }
        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPassword.Text = PasswordBox.Password;

            PasswordBox.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
        }

        private void chkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = txtPassword.Text;

            txtPassword.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
        }
    }
}
