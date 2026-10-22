using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry
{
    public class TelemetryEventName
    {
        // Application Lifecycle
        public const string ApplicationStarted =
            "ApplicationLaunch";

        public const string ApplicationReady =
            "ApplicationReady";

        public const string ApplicationClosed =
            "ApplicationClosed";

        public const string ApplicationCrashed =
            "ApplicationCrashed";

        // System Lifecycle
        public const string SystemSleep =
            "SystemSleep";

        public const string SystemResume =
            "SystemResume";

        // AI Service
        public const string AIRequestStarted =
            "AIRequestStarted";

        public const string AIRequestCompleted =
            "AIRequestCompleted";

        public const string AIRequestFailed =
            "AIRequestFailed";

        // User Actions

        public const string ChatMessageSent =
            "ChatMessageSent";

        public const string FileAttachmentAdded =
            "FileAttachmentAdded";

        public const string NewChatStarted =
            "NewChatStarted";

        public const string ChatRenamed =
            "ChatRenamed";

        public const string ChatDeleted =
            "ChatDeleted";

        public const string ChatCleared =
            "ChatCleared";

        public const string SettingsOpened =
            "SettingsOpened";

        public const string UserLoggedIn =
            "UserLoggedIn";

        public const string UserLoggedOut =
            "UserLoggedOut";

        public const string MicrophoneButtonClicked =
            "MicrophoneButtonClicked";

        public const string AutoLaunchToggled =
            "AutoLaunchToggled";

        public const string ThemeChanged =
            "ThemeChanged";

        public const string NotificationToggled = "NotificationToggled";

        public const string GoogleLoginButtonClicked = "GoogleLoginButtonClicked";
    }
}


