using POC12_LoginApplicationWith_MVVM.Commands;
using POC12_LoginApplicationWith_MVVM.Models;
using POC12_LoginApplicationWith_MVVM.Services;
using POC12_LoginApplicationWith_MVVM.ViewsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Messaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POC12_LoginApplicationWith_MVVM.Views
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private bool _isAuthenticated;
        private string _message;
        private string _usernameError;
        private string _passwordError;
        private bool success;
        private readonly IAuthenticationService _authenticationService;



        public ICommand LoginCommand { get; }

        public ICommand CancelCommand { get; }


        public MainViewModel()
        {
            LoginCommand = new RelayCommand(Login, CanLogin);

            CancelCommand = new RelayCommand(Cancel);

            _authenticationService = new AuthenticationService();

            LoginCommand = new RelayCommand(Login);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Login()
        {
            try
            {

                if (!LoginValidate())
                    return;

                bool isAuthenticated = _authenticationService.Authenticate(Username,Password);

                if (isAuthenticated)
                {
                    HomeView home = new HomeView();
                    home.DataContext = new HomeViewModel(Username);
                    home.Show();

                    Application.Current.Windows[0].Close();
                }
                else
                {
                    Message = "Invalid Username or Password";
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }


        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set
            {
                _isAuthenticated = value;
                OnPropertyChanged();
            }
        }

        public string UsernameError
        {
            get => _usernameError;
            set
            {
                _usernameError = value;
                OnPropertyChanged();
            }
        }

        public string PasswordError
        {
            get => _passwordError;
            set
            {
                _passwordError = value;
                OnPropertyChanged();
            }
        }


        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }


        private bool CanLogin()
        {
            return true;

        }

        private void Cancel()
        {
            Username = "";
            Password = "";
            Message = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public bool LoginValidate()
        {
          
                UsernameError = string.Empty;
                PasswordError = string.Empty;
                Message = string.Empty;

                bool isValid = true;

                if (string.IsNullOrWhiteSpace(Username))
                {
                    UsernameError = "Username is required.";
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    PasswordError = "Password is required.";
                    isValid = false;
                }

                else if (Password.Length < 6)
                {
                    PasswordError = "Password must be at least 6 characters long.";
                    isValid = false;
                }

                return isValid;

            }
          
        }
    }

            

        
    


