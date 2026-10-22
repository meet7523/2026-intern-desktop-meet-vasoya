using POC16_AICHATAPPLICATION_FIGMATOWPF.Data;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Services;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF
{
    public partial class App : Application
    {
        private SystemMonitoringService _systemMonitoringService;
        public bool IsStartedFromWindowsStartup
        {
            get;
            private set;
        }

        private NotificationService _notificationService;

        private SingleInstanceService _singleInstanceService;

        private bool _pendingDeepLink;

        public string DeepLink
        {
            get;
            private set;
        }

        protected override void OnStartup(
            StartupEventArgs e)
        {
            base.OnStartup(e);

            TelemetryService.Instance.TrackEvent(
            TelemetryEventName.ApplicationStarted,
            TelemetryEventCategory.ApplicationLifecycle);


            DispatcherUnhandledException +=
                OnDispatcherUnhandledException;

            TaskScheduler.UnobservedTaskException +=
                OnUnobservedTaskException;

            AppDomain.CurrentDomain.UnhandledException +=
                OnAppDomainUnhandledException;

            _systemMonitoringService = new SystemMonitoringService();
            _systemMonitoringService.Start();

            // ==================================================
            // 1. CREATE SINGLE INSTANCE SERVICE FIRST
            // ==================================================

            _singleInstanceService =
                new SingleInstanceService();

            bool isFirstInstance =
                _singleInstanceService.IsFirstInstance();

            // ==================================================
            // 2. READ DEEP LINK
            // ==================================================

            DeepLink =
                DeepLinkService.GetDeepLink(e.Args);

            // ==================================================
            // 3. SECOND INSTANCE
            // ==================================================

            if (!isFirstInstance)
            {
                if (!string.IsNullOrWhiteSpace(DeepLink))
                {
                    _singleInstanceService
                        .SendDeepLinkToRunningInstance(
                            DeepLink);
                }

                _singleInstanceService.Dispose();

                Shutdown();

                return;
            }

            // ==================================================
            // 4. FIRST INSTANCE ONLY
            // ==================================================

            DeepLinkService.RegisterProtocol();

            _singleInstanceService
                .StartDeepLinkListener(
                    OnDeepLinkReceived);

            // ==================================================
            // 5. WINDOWS STARTUP DETECTION
            // ==================================================

            IsStartedFromWindowsStartup =
                e.Args.Any(arg =>
                    arg.Equals(
                        "--startup",
                        StringComparison.OrdinalIgnoreCase));

            // ==================================================
            // 6. STARTUP NOTIFICATION
            // ==================================================

            ShowStartupNotification();

            // ==================================================
            // 7. SHUTDOWN MODE
            // ==================================================

            ShutdownMode =
                ShutdownMode.OnExplicitShutdown;

            // ==================================================
            // 8. DATABASE
            // ==================================================

            DatabaseService databaseService =
                new DatabaseService();

            databaseService.InitializeDatabase();

            // ==================================================
            // 9. THEME
            // ==================================================

            var settingsService =
                new AppSettingsService();

            var settings =
                settingsService.Load();

            ThemeService.ApplyTheme(
                settings.Theme);

            // ==================================================
            // 10. NORMAL STARTUP
            // ==================================================

            if (string.IsNullOrWhiteSpace(DeepLink))
            {
                OpenLoginWindow();

                return;
            }

            // ==================================================
            // 11. DEEP LINK STARTUP
            // ==================================================

            HandleDeepLink(DeepLink);

            // ==================================================
            // 12. COLD START
            // ==================================================

            if (Application.Current.MainWindow == null)
            {
                OpenLoginWindow();
            }
        }

        // ======================================================
        // SECOND INSTANCE → FIRST INSTANCE
        // ======================================================

        private void OnDeepLinkReceived(
            string deepLink)
        {
            Dispatcher.Invoke(() =>
            {
                HandleDeepLink(deepLink);
            });
        }

        // ======================================================
        // HANDLE openchatai://...
        // ======================================================

        private void HandleDeepLink(
    string deepLink)
        {
            if (!DeepLinkService.IsDeepLink(
                deepLink))
            {
                return;
            }

            try
            {
                Uri uri =
                    new Uri(deepLink);

                if (!uri.Host.Equals(
                    "login",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                // ==================================================
                // MainWindow already exists
                // ==================================================

                MainWindow mainWindow =
                    Application.Current.MainWindow
                    as MainWindow;

                if (mainWindow != null)
                {
                    ActivateMainWindow();

                    return;
                }

                // ==================================================
                // LoginWindow is still open
                //
                // Remember the deep link.
                // OpenMainWindow() will activate it after login.
                // ==================================================

                _pendingDeepLink = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Deep link handling failed: " +
                    ex);
            }
        }
        // ======================================================
        // LOGIN WINDOW
        // ======================================================

        private void OpenLoginWindow()
        {
            LoginWindow loginWindow =
                new LoginWindow();

            bool? result =
                loginWindow.ShowDialog();

            if (result == true)
            {
                OpenMainWindow();

                return;
            }

            Shutdown();
        }

        // ======================================================
        // MAIN WINDOW
        // ======================================================

        private void OpenMainWindow()
        {
            MainWindow mainWindow =
                new MainWindow();

            Application.Current.MainWindow =
                mainWindow;

            ShutdownMode =
                ShutdownMode.OnMainWindowClose;

            mainWindow.Show();

            TelemetryService.Instance.TrackEvent(
            TelemetryEventName.ApplicationReady,
            TelemetryEventCategory.ApplicationLifecycle);

            // ==============================================
            // Deep link arrived while LoginWindow was open
            // ==============================================

            if (_pendingDeepLink)
            {
                _pendingDeepLink = false;

                ActivateMainWindow();
            }
        }

        // ======================================================
        // ACTIVATE EXISTING MAIN WINDOW
        // ======================================================

        private void ActivateMainWindow()
        {
            MainWindow mainWindow =
                Application.Current.MainWindow
                as MainWindow;

            if (mainWindow == null)
            {
                return;
            }

            if (mainWindow.WindowState ==
                WindowState.Minimized)
            {
                mainWindow.WindowState =
                    WindowState.Normal;
            }

            if (!mainWindow.IsVisible)
            {
                mainWindow.Show();
            }

            mainWindow.Activate();

            mainWindow.Topmost = true;
            mainWindow.Topmost = false;

            mainWindow.Focus();
        }

        // ======================================================
        // WINDOWS STARTUP NOTIFICATION
        // ======================================================

        private void ShowStartupNotification()
        {
            if (!IsStartedFromWindowsStartup)
            {
                return;
            }

            bool notificationEnabled =
                POC16_AICHATAPPLICATION_FIGMATOWPF
                    .Properties
                    .Settings
                    .Default
                    .NotificationsEnabled;

            if (!notificationEnabled)
            {
                return;
            }

            _notificationService =
                new NotificationService();

            _notificationService
                .ShowAutoLaunchNotification();
        }

        // ======================================================
        // APPLICATION EXIT
        // ======================================================

        protected override void OnExit(
            ExitEventArgs e)
        {
            TelemetryService.Instance.TrackEvent(
            TelemetryEventName.ApplicationClosed,
            TelemetryEventCategory.ApplicationLifecycle);

            _notificationService?.Dispose();

            _singleInstanceService?.Dispose();

            if (_systemMonitoringService != null)
            {
                _systemMonitoringService.Stop();
                _systemMonitoringService.Dispose();
                _systemMonitoringService = null;
            }

            base.OnExit(e);
        }


        private void OnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
        {
            TelemetryService.Instance.TrackException(
                TelemetryEventName.ApplicationCrashed,
                TelemetryEventCategory.Error,
                e.Exception,
                new Dictionary<string, object>
                {
            { "ExceptionSource", "DispatcherUnhandledException" }
                });

            e.Handled = false;
        }


        private void OnUnobservedTaskException(
        object sender,
        UnobservedTaskExceptionEventArgs e)
        {
            TelemetryService.Instance.TrackException(
                TelemetryEventName.ApplicationCrashed,
                TelemetryEventCategory.Error,
                e.Exception,
                new Dictionary<string, object>
                {
            { "ExceptionSource", "TaskScheduler.UnobservedTaskException" }
                });

            e.SetObserved();
        }


        private void OnAppDomainUnhandledException(
        object sender,
        UnhandledExceptionEventArgs e)
        {
            Exception exception =
                e.ExceptionObject as Exception;

            if (exception == null)
            {
                exception = new Exception(
                    "Unhandled exception object was not an Exception.");
            }

            TelemetryService.Instance.TrackException(
                TelemetryEventName.ApplicationCrashed,
                TelemetryEventCategory.Error,
                exception,
                new Dictionary<string, object>
                {
            { "ExceptionSource", "AppDomain.UnhandledException" },
            { "IsTerminating", e.IsTerminating }
                });
        }
    }
}