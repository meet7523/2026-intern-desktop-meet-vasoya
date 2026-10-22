using System.Windows;
using System.Windows.Controls;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnBoundPasswordChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(PasswordBoxHelper));

        public static string GetBoundPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(
            DependencyObject obj,
            string value)
        {
            obj.SetValue(BoundPasswordProperty, value);
        }

        public static bool GetAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(AttachProperty);
        }

        public static void SetAttach(
            DependencyObject obj,
            bool value)
        {
            obj.SetValue(AttachProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(
            DependencyObject obj,
            bool value)
        {
            obj.SetValue(IsUpdatingProperty, value);
        }

        private static void Attach(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = d as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(
            object sender,
            RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            SetIsUpdating(passwordBox, true);

            SetBoundPassword(
                passwordBox,
                passwordBox.Password);

            SetIsUpdating(passwordBox, false);
        }

        private static void OnBoundPasswordChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = d as PasswordBox;

            if (passwordBox == null)
                return;

            if (GetIsUpdating(passwordBox))
                return;

            string newPassword = e.NewValue as string ?? string.Empty;

            if (passwordBox.Password != newPassword)
            {
                SetIsUpdating(passwordBox, true);

                passwordBox.Password = newPassword;

                SetIsUpdating(passwordBox, false);
            }
        }
    }
}