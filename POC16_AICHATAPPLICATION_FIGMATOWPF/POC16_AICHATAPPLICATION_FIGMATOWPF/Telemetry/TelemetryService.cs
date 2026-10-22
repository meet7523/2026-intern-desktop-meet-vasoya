using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry
{
    public sealed class TelemetryService
    {
        private static readonly Lazy<TelemetryService> _instance =
            new Lazy<TelemetryService>(() => new TelemetryService());

        private readonly object _lock = new object();

        private readonly string _sessionId;
        private readonly string _telemetryDirectory;
        private readonly string _telemetryFilePath;
        private readonly string _applicationVersion;
        private readonly int _processId;

        // =========================================================
        // NEW: Store telemetry events in memory
        // =========================================================

        private readonly List<TelemetryEvent> _telemetryEvents;

        public static TelemetryService Instance
        {
            get { return _instance.Value; }
        }

        private TelemetryService()
        {
            _sessionId = Guid.NewGuid().ToString("N");

            _processId = Process.GetCurrentProcess().Id;

            _applicationVersion = GetApplicationVersion();

            _telemetryDirectory = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "POC16_AICHATAPPLICATION_FIGMATOWPF",
                "Telemetry");

            _telemetryFilePath = CreateTelemetryFilePath();

            // =====================================================
            // NEW: Initialize telemetry event collection
            // =====================================================

            _telemetryEvents = new List<TelemetryEvent>();

            // Create the initial valid JSON structure
            InitializeTelemetryFile();
        }

        public string SessionId
        {
            get { return _sessionId; }
        }

        public string TelemetryFilePath
        {
            get { return _telemetryFilePath; }
        }

        // =========================================================
        // Track Event
        // =========================================================

        public void TrackEvent(
            string eventName,
            TelemetryEventCategory category,
            IDictionary<string, object> properties = null)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return;
            }

            try
            {
                var eventProperties =
                    properties != null
                        ? new Dictionary<string, object>(properties)
                        : new Dictionary<string, object>();

                var telemetryEvent = new TelemetryEvent
                {
                    EventName = eventName,

                    Category = category.ToString(),

                    TimestampUtc = DateTime.UtcNow,

                    SessionId = _sessionId,

                    ApplicationVersion = _applicationVersion,

                    ProcessId = _processId,

                    Properties = eventProperties,
                };

                lock (_lock)
                {
                    // =================================================
                    // NEW:
                    // Add event to the in-memory collection
                    // =================================================

                    _telemetryEvents.Add(telemetryEvent);

                    // =================================================
                    // NEW:
                    // Serialize the complete collection as JSON
                    // =================================================

                    string json = JsonConvert.SerializeObject(
                        _telemetryEvents,
                        Formatting.Indented);

                    // =================================================
                    // NEW:
                    // Write complete JSON array to file
                    // =================================================

                    File.WriteAllText(
                        _telemetryFilePath,
                        json);
                }
            }
            catch
            {
                // Telemetry must never crash the application.
            }
        }

        // =========================================================
        // Track Exception
        // =========================================================

        public void TrackException(
            string eventName,
            TelemetryEventCategory category,
            Exception exception,
            IDictionary<string, object> properties = null)
        {
            if (exception == null)
            {
                return;
            }

            var eventProperties =
                properties != null
                    ? new Dictionary<string, object>(properties)
                    : new Dictionary<string, object>();

            eventProperties["ExceptionType"] =
                exception.GetType().FullName;

            eventProperties["ExceptionMessage"] =
                exception.Message;

            eventProperties["ExceptionStackTrace"] =
                exception.StackTrace ?? string.Empty;

            if (exception.InnerException != null)
            {
                eventProperties["InnerExceptionType"] =
                    exception.InnerException.GetType().FullName;

                eventProperties["InnerExceptionMessage"] =
                    exception.InnerException.Message;
            }

            TrackEvent(
                eventName,
                category,
                eventProperties);
        }

        // =========================================================
        // Initialize Telemetry File
        // =========================================================

        private void InitializeTelemetryFile()
        {
            try
            {
                Directory.CreateDirectory(
                    _telemetryDirectory);

                // Create an empty JSON array.
                File.WriteAllText(
                    _telemetryFilePath,
                    "[]");
            }
            catch
            {
                // Telemetry must never crash the application.
            }
        }

        // =========================================================
        // Create Telemetry File
        // =========================================================

        private string CreateTelemetryFilePath()
        {
            try
            {
                Directory.CreateDirectory(
                    _telemetryDirectory);

                string timestamp =
                    DateTime.Now.ToString(
                        "yyyyMMdd_HHmmss_fff");

                return Path.Combine(
                    _telemetryDirectory,
                    "Telemetry_"
                    + timestamp
                    + "_"
                    + _sessionId
                    + ".json");
            }
            catch
            {
                return Path.Combine(
                    Path.GetTempPath(),
                    "Telemetry_"
                    + _sessionId
                    + ".json");
            }
        }

        // =========================================================
        // Application Version
        // =========================================================

        private static string GetApplicationVersion()
        {
            try
            {
                Assembly assembly =
                    Assembly.GetEntryAssembly();

                if (assembly != null)
                {
                    Version version =
                        assembly.GetName().Version;

                    if (version != null)
                    {
                        return version.ToString();
                    }
                }
            }
            catch
            {
                // Use fallback value.
            }

            return "Unknown";
        }
    }
}