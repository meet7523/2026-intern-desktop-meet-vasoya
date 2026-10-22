using Microsoft.Win32;
using System;
using System.Timers;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public sealed class SystemMonitoringService : IDisposable
    {
        private readonly Timer _runtimeTimer;

        private bool _isStarted;
        private bool _firstRuntimeLogWritten;
        private bool _disposed;

        // First runtime log after 3 minutes
        private static readonly TimeSpan FirstLogInterval =
            TimeSpan.FromMinutes(3);

        // Subsequent runtime logs every 10 minutes
        private static readonly TimeSpan RecurringLogInterval =
            TimeSpan.FromMinutes(10);

        public SystemMonitoringService()
        {
            _runtimeTimer = new Timer
            {
                AutoReset = false
            };

            _runtimeTimer.Elapsed += RuntimeTimerElapsed;

            SystemEvents.PowerModeChanged +=
                SystemEvents_PowerModeChanged;
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(
                    nameof(SystemMonitoringService));

            if (_isStarted)
                return;

            _isStarted = true;

            ApplicationLogger.Log(
    "Application Started - System Monitoring Initialized");

            // First log must happen after 3 minutes.
            _firstRuntimeLogWritten = false;

            _runtimeTimer.Interval =
                FirstLogInterval.TotalMilliseconds;

            _runtimeTimer.Start();
        }

        private void RuntimeTimerElapsed(
            object sender,
            ElapsedEventArgs e)
        {
            if (_disposed || !_isStarted)
                return;

            try
            {
                if (!_firstRuntimeLogWritten)
                {
                    _firstRuntimeLogWritten = true;

                    ApplicationLogger.Log(
                        "Application Runtime Log - First entry after 3 minutes");

                    // Change timer to 10-minute interval.
                    _runtimeTimer.Interval =
                        RecurringLogInterval.TotalMilliseconds;
                }
                else
                {
                    ApplicationLogger.Log(
                        "Application Runtime Log - 10 minute interval");
                }

                // Timer is AutoReset=false,
                // therefore explicitly start it again.
                if (!_disposed && _isStarted)
                {
                    _runtimeTimer.Start();
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(
                    "Runtime monitoring error: " + ex.Message);

                if (!_disposed && _isStarted)
                {
                    _runtimeTimer.Start();
                }
            }
        }

        private void SystemEvents_PowerModeChanged(
            object sender,
            PowerModeChangedEventArgs e)
        {
            if (_disposed)
                return;

            try
            {
                switch (e.Mode)
                {
                    case PowerModes.Suspend:

                        ApplicationLogger.Log(
                            "System Sleep Detected");

                        TelemetryService.Instance.TrackEvent(
                            TelemetryEventName.SystemSleep,
                            TelemetryEventCategory.System);

                        break;

                    case PowerModes.Resume:

                        ApplicationLogger.Log(
                            "System Resume Detected");

                        TelemetryService.Instance.TrackEvent(
                           TelemetryEventName.SystemResume,
                           TelemetryEventCategory.System);

                        break;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(
                    "Power monitoring error: " + ex.Message);
            }
        }

        public void Stop()
        {
            if (!_isStarted)
                return;

            _isStarted = false;

            _runtimeTimer.Stop();

            ApplicationLogger.Log(
                "Application Monitoring Stopped");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _runtimeTimer.Stop();
            _runtimeTimer.Elapsed -= RuntimeTimerElapsed;

            SystemEvents.PowerModeChanged -=
                SystemEvents_PowerModeChanged;

            _runtimeTimer.Dispose();
        }
    }
}