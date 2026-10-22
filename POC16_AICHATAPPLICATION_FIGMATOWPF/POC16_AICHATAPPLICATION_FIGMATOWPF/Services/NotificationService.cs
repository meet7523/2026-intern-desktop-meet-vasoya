using System;
using System.Drawing;
using System.Windows.Forms;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public class NotificationService : IDisposable
    {
        private NotifyIcon _notifyIcon;

        public NotificationService()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true
            };
        }

        public void ShowAutoLaunchNotification()
        {
            if (_notifyIcon == null)
                return;

            _notifyIcon.BalloonTipTitle = "AI Chat Application";

            _notifyIcon.BalloonTipText =
                "Application started automatically with Windows.";

            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;

            _notifyIcon.ShowBalloonTip(5000);
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }
    }
}