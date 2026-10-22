using POC16_AICHATAPPLICATION_FIGMATOWPF.Services;
using POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels;
using System;
using System.Windows;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Views
{
    public partial class AiChatView
    {
        private AiChatViewModel _viewModel;

        public AiChatView()
        {
            InitializeComponent();

            Loaded += AiChatView_Loaded;
            Unloaded += AiChatView_Unloaded;
        }

        private void AiChatView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AiChatViewModel viewModel)
            {
                _viewModel = viewModel;
                _viewModel.LogoutRequested += OnLogoutRequested;
            }
        }

        private void AiChatView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.LogoutRequested -= OnLogoutRequested;
                _viewModel = null;
            }
        }

        private void OnLogoutRequested()
        {
            Window oldMainWindow = Window.GetWindow(this);

            if (oldMainWindow == null)
                return;

            // 1. Clear currently authenticated Google user
            CurrentUserSession.Clear();

            // 2. Hide current MainWindow
            oldMainWindow.Hide();

            // 3. Open Google Login page
            LoginWindow loginWindow = new LoginWindow();

            bool? loginResult = loginWindow.ShowDialog();

            // 4. If Google login succeeds
            if (loginResult == true)
            {
                // Create a completely fresh MainWindow
                MainWindow newMainWindow = new MainWindow();

                Application.Current.MainWindow = newMainWindow;

                newMainWindow.Show();

                // Close old MainWindow
                oldMainWindow.Close();
            }
            else
            {
                // User closed LoginWindow without logging in.
                // Close old MainWindow, but DO NOT call Application.Current.Shutdown()
                oldMainWindow.Close();

                // Keep application alive only if another window exists.
                if (Application.Current.MainWindow == oldMainWindow)
                {
                    Application.Current.MainWindow = null;
                }
            }
        }
    }
    }
