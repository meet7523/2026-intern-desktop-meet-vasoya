using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public int ChatId { get; set; }

        public string SenderType { get; set; } = string.Empty;

        public string MessageText { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string AttachmentFileName { get; set; }

        public string AttachmentExtension { get; set; }

        public string AttachmentSize { get; set; }

        public byte[] AttachmentData { get; set; }
    }
}
