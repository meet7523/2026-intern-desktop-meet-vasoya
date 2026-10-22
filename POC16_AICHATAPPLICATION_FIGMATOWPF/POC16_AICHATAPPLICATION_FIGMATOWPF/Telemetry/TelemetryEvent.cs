using System;
using System.Collections.Generic;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry
{
    public class TelemetryEvent
    {
        public string EventName { get; set; }

        public string  Category { get; set; }

        public DateTime TimestampUtc { get; set; }

        public string SessionId { get; set; }

        public string ApplicationVersion { get; set; }

        public int ProcessId { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        public Dictionary<string, object> Metadata { get; set; }
    }
}