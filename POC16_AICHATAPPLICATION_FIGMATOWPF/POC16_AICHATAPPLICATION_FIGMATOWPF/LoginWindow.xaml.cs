using POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels;
using System;
using System.Windows;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            _viewModel = new LoginViewModel();

            DataContext = _viewModel;

            _viewModel.LoginSucceeded += OnLoginSucceeded;
        }

        private void OnLoginSucceeded()
        {
            DialogResult = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.LoginSucceeded -= OnLoginSucceeded;

            base.OnClosed(e);
        }
    }
}