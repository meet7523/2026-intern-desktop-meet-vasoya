using System;
using System.Configuration;
using System.Windows;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public static class GoogleOAuthConfiguration
    {
        public static string ClientId
        {
            get
            {
                string clientId =
                    ConfigurationManager.AppSettings[
                        "GoogleOAuthClientId"];

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    MessageBox.Show(
                        "GoogleOAuthClientId is not configured.",
                        "Google OAuth Configuration",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    throw new InvalidOperationException(
                        "Google OAuth Client ID is not configured.");
                }

                return clientId.Trim();
            }
        }

        public static string ClientSecret
        {
            get
            {
                string clientSecret =
                    ConfigurationManager.AppSettings[
                        "GoogleOAuthClientSecret"];

                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    MessageBox.Show(
                        "GoogleOAuthClientSecret is not configured.",
                        "Google OAuth Configuration",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    throw new InvalidOperationException(
                        "Google OAuth Client Secret is not configured.");
                }

                return clientSecret.Trim();
            }
        }
    }
}