using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public class GeminiApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public GeminiApiException(
            HttpStatusCode statusCode,
            string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
