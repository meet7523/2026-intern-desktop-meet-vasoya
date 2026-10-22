using POC16_AICHATAPPLICATION_FIGMATOWPF.Commands;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Data;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Helpers;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Services;
using POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static POC16_AICHATAPPLICATION_FIGMATOWPF.Services.GoogleOAuthService;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry;


namespace POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly ChatRepository _chatRepository;
        private readonly INavigationService _navigationService;
        private readonly GoogleOAuthService _googleOAuthService;


        public event Action LoginSucceeded;

        private bool _isPasswordLengthValid;
        private bool _isUppercaseValid;
        private bool _isLowercaseValid;
        private bool _isNumberValid;
        private bool _isSpecialCharacterValid;
        private bool _isNoWhitespaceValid;

        private string _errorMessage = string.Empty;
        private bool _isBusy;

        private string _userId = string.Empty;
        private string _username = string.Empty;


        public string UserId
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();

                    UpdateLoginButtonState();
                    OnPropertyChanged(nameof(CanLogin));
                }
            }
        }


        private string _password = string.Empty;

        public string Password
        {
            get => _password;
            set
            {
                if (_password == value)
                    return;

                _password = value;

                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(HasPassword));
                OnPropertyChanged(nameof(CanLogin));

                ValidatePassword();
                UpdatePasswordErrorMessage();
            }
        }


        public bool HasPassword =>
    !string.IsNullOrWhiteSpace(Password);



        private bool _isPasswordVisible;

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                if (_isPasswordVisible == value)
                    return;

                _isPasswordVisible = value;

                OnPropertyChanged(nameof(IsPasswordVisible));
            }
        }



        public bool IsPasswordLengthValid
        {
            get => _isPasswordLengthValid;
            private set
            {
                if (_isPasswordLengthValid != value)
                {
                    _isPasswordLengthValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsUppercaseValid
        {
            get => _isUppercaseValid;
            private set
            {
                if (_isUppercaseValid != value)
                {
                    _isUppercaseValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLowercaseValid
        {
            get => _isLowercaseValid;
            private set
            {
                if (_isLowercaseValid != value)
                {
                    _isLowercaseValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNumberValid
        {
            get => _isNumberValid;
            private set
            {
                if (_isNumberValid != value)
                {
                    _isNumberValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSpecialCharacterValid
        {
            get => _isSpecialCharacterValid;
            private set
            {
                if (_isSpecialCharacterValid != value)
                {
                    _isSpecialCharacterValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNoWhitespaceValid
        {
            get => _isNoWhitespaceValid;
            private set
            {
                if (_isNoWhitespaceValid != value)
                {
                    _isNoWhitespaceValid = value;
                    OnPropertyChanged();
                }
            }
        }



        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

      
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();

                    UpdateLoginButtonState();
                    OnPropertyChanged(nameof(CanLogin));
                }
            }
        }


        public bool CanLogin
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserId)
                    && !string.IsNullOrWhiteSpace(Password)
                    && !IsBusy;
            }
        }


        private void ValidatePassword()
        {
            string password = Password ?? string.Empty;

            IsPasswordLengthValid = password.Length >= 8;

            IsUppercaseValid = password.Any(char.IsUpper);

            IsLowercaseValid = password.Any(char.IsLower);

            IsNumberValid = password.Any(char.IsDigit);

            IsSpecialCharacterValid =
                password.Any(ch =>
                    !char.IsLetterOrDigit(ch) &&
                    !char.IsWhiteSpace(ch));

            IsNoWhitespaceValid =
                !password.Any(char.IsWhiteSpace);
        }

        private void UpdateLoginButtonState()
        {
            OnPropertyChanged(nameof(CanLogin));
        }

        private void UpdatePasswordErrorMessage()
        {
            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = string.Empty;
                return;
            }

            if (!IsPasswordLengthValid)
            {
                ErrorMessage = "Password must contain at least 8 characters.";
                return;
            }

            if (!IsUppercaseValid)
            {
                ErrorMessage = "Password must contain at least one uppercase letter.";
                return;
            }

            if (!IsLowercaseValid)
            {
                ErrorMessage = "Password must contain at least one lowercase letter.";
                return;
            }

            if (!IsNumberValid)
            {
                ErrorMessage = "Password must contain at least one number.";
                return;
            }

            if (!IsSpecialCharacterValid)
            {
                ErrorMessage = "Password must contain at least one special character.";
                return;
            }

            if (!IsNoWhitespaceValid)
            {
                ErrorMessage = "Password cannot contain spaces.";
                return;
            }

            ErrorMessage = string.Empty;
        }


        public ICommand LoginCommand { get; }

        public ICommand TogglePasswordVisibilityCommand { get; }

        public ICommand GoogleLoginCommand { get; }

        public LoginViewModel()
        
        {
            _databaseService = new DatabaseService();

            LoginCommand = new RelayCommand(_ => Login());

            TogglePasswordVisibilityCommand = new RelayCommand(TogglePasswordVisibility);

            _chatRepository = new ChatRepository(_databaseService);

            _navigationService = new NavigationService();

            GoogleLoginCommand = new RelayCommand(ExecuteGoogleLogin);

            _googleOAuthService = new GoogleOAuthService();

        }


        private async void ExecuteGoogleLogin(object parameter)
        {
            TelemetryService.Instance.TrackEvent(
           TelemetryEventName.GoogleLoginButtonClicked,
           TelemetryEventCategory.UserAction,
           new Dictionary<string, object>
           {
                { "SourceTrigger", "LoginView" },
                { "ButtonType", "Button" }
           });


            try
            {
                ErrorMessage = string.Empty;

                GoogleUserInfo googleUser =
                    await _googleOAuthService.StartGoogleLoginAsync();

                if (googleUser == null)
                {
                    ErrorMessage = "Google authentication failed.";
                    return;
                }

                int userId = _databaseService.SaveOrUpdateGoogleUser(
                    googleUser.GoogleUserId,
                    googleUser.Name,
                    googleUser.Email,
                    googleUser.Picture);

                AuthenticatedUser user = new AuthenticatedUser
                {
                    Id = userId,
                    GoogleUserId = googleUser.GoogleUserId,
                    Name = googleUser.Name,
                    Email = googleUser.Email,
                    ProfilePicture = googleUser.Picture,
                    EmailVerified = googleUser.EmailVerified
                };

                // VERY IMPORTANT
                CurrentUserSession.SetUser(user);

                TelemetryService.Instance.TrackEvent(
                TelemetryEventName.UserLoggedIn,
                TelemetryEventCategory.UserAction,
                new Dictionary<string, object>
                {
                    { "LoginMethod", "Google" },
                    { "SourceTrigger", "LoginView" },
                    { "ButtonType", "Button" }
                });

                LoginSucceeded?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        private void TogglePasswordVisibility(object parameter)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private void Login()
        {
            string userId = UserId;
            string password = Password;


            // Validate empty fields
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Please enter User ID and Password.";
                return;
            }

            DatabaseService databaseService = new DatabaseService();

            bool isValid = databaseService.ValidateUser(userId, password);

            if (isValid)
            {
                ErrorMessage = string.Empty;

                TelemetryService.Instance.TrackEvent(
                TelemetryEventName.UserLoggedIn,
                TelemetryEventCategory.UserAction,
                new Dictionary<string, object>
                {
                    { "LoginMethod", "UserIdPassword" },
                    { "SourceTrigger", "LoginView" },
                    { "ButtonType", "Button" }
                });

                // Notify LoginWindow that login was successful
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = "Invalid Username or Password.";
            }
        }

    }
}

