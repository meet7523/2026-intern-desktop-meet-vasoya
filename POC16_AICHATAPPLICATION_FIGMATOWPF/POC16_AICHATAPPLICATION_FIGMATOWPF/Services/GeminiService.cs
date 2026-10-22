using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry;
using System.Diagnostics;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    
    public class GeminiService
    {

        private readonly HttpClient _httpClient;
        private int _timeoutSeconds = 60;
        private const string Model =
            "gemini-3.1-flash-lite";

        private const string Endpoint =
            "https://generativelanguage.googleapis.com/v1beta/models/"
            + Model
            + ":generateContent";

        public GeminiService()
        {
            _httpClient = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

      
        

        public int TimeoutSeconds
        {
            get
            {
                return _timeoutSeconds;
            }
        }

        public void SetTimeout(int seconds)
        {
            if (seconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(seconds));

            _timeoutSeconds = seconds;
        }

        Stopwatch requestStopwatch = Stopwatch.StartNew();

        public async Task<string> SendMessageAsync(
            string userMessage,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userMessage))
                {
                    throw new ArgumentException(
                        "Message cannot be empty.",
                        nameof(userMessage));
                }

                TelemetryService.Instance.TrackEvent(
                    TelemetryEventName.AIRequestStarted,
                    TelemetryEventCategory.AIService,
                    new Dictionary<string, object>
                    {
                    { "RequestType", "Text" },
                    { "HasAttachment", false }
                    });

               
                string apiKey =
                    Environment.GetEnvironmentVariable("GEMINI_API_KEY");

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException(
                        "GEMINI_API_KEY environment variable is not configured.");
                }


                int requestTimeoutSeconds = _timeoutSeconds;

                CancellationTokenSource timeoutCts =
                   new CancellationTokenSource(
                       TimeSpan.FromSeconds(requestTimeoutSeconds));

                CancellationTokenSource linkedCts =
                   CancellationTokenSource.CreateLinkedTokenSource(
                       cancellationToken,
                       timeoutCts.Token);

                CancellationToken requestToken =
                    linkedCts.Token;

                // Instructions for Gemini response formatting
                var requestBody = new
                {
                    systemInstruction = new
                    {
                        parts = new[]
                        {
                        new
                        {
                            text =
                                "You are a helpful AI assistant for a student "
                                + "learning application. "
                                + "Answer the user's question clearly and accurately. "
                                + "Use clean plain text formatting. "
                                + "Do not use Markdown headings such as # or ##. "
                                + "Do not use Markdown bold or italic formatting "
                                + "such as **text** or *text*. "
                                + "Do not use decorative symbols or unnecessary emojis. "
                                + "For lists, use simple numbered lists such as "
                                + "1., 2., 3. or bullet points. "
                                + "For programming questions, preserve all programming "
                                + "symbols and provide readable code."
                        }
                    }
                    },

                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = userMessage
                            }
                        }
                    }
                }
                };

                string json =
                    JsonSerializer.Serialize(requestBody);

                string url =
                    $"{Endpoint}?key={apiKey}";

                var content = new StringContent(
                   json,
                   Encoding.UTF8,
                   "application/json");


                HttpResponseMessage response;

                try
                {
                    response = await _httpClient.PostAsync(
                    url,
                    content,
                    requestToken);
                }
                catch (OperationCanceledException)
        when (timeoutCts.IsCancellationRequested &&
              !cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException(
                        $"The Gemini request timed out after " +
                        $"{_timeoutSeconds} seconds.");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }


                string responseJson =
        await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new GeminiApiException(
                        response.StatusCode,
                        $"Gemini API request failed with status code " +
                        $"{(int)response.StatusCode} ({response.ReasonPhrase}).");
                }

                JsonDocument document =
                    JsonDocument.Parse(responseJson);

                JsonElement root =
                    document.RootElement;

                if (root.TryGetProperty(
                        "candidates",
                        out JsonElement candidates))
                {
                    if (candidates.GetArrayLength() > 0)
                    {
                        JsonElement candidate =
                            candidates[0];

                        if (candidate.TryGetProperty(
                                "content",
                                out JsonElement contentElement))
                        {
                            if (contentElement.TryGetProperty(
                                    "parts",
                                    out JsonElement parts))
                            {
                                foreach (JsonElement part
                                         in parts.EnumerateArray())
                                {
                                    if (part.TryGetProperty(
                                            "text",
                                            out JsonElement text))
                                    {
                                        string result =
                                            text.GetString();

                                        if (!string.IsNullOrWhiteSpace(result))
                                        {

                                            requestStopwatch.Stop();

                                            TelemetryService.Instance.TrackEvent(
                                                TelemetryEventName.AIRequestCompleted,
                                                TelemetryEventCategory.AIService,
                                                new Dictionary<string, object>
                                                {
                                                { "RequestType", "Text" },
                                                { "HasAttachment", false },
                                                { "DurationMs", requestStopwatch.ElapsedMilliseconds }
                                                });

                                            // Clean Gemini Markdown
                                            return CleanResponse(result);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                throw new InvalidOperationException(
                    "Gemini returned an empty or unexpected response.\n\n" +
                    responseJson);
            }
            catch (Exception ex)
            {
                requestStopwatch.Stop();

                TelemetryService.Instance.TrackException(
                TelemetryEventName.AIRequestFailed,
                TelemetryEventCategory.AIService,
                ex,
                new Dictionary<string, object>
                {
                    { "RequestType", "Text" },
                    { "HasAttachment", false },
                    { "DurationMs", requestStopwatch.ElapsedMilliseconds }
                });

                throw;
            }
        }


        public async Task<string> SendMessageWithAttachmentAsync(
    string userMessage,
    string filePath,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(
                    "Attachment file path cannot be empty.",
                    nameof(filePath));
            }

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "The attached file could not be found.",
                    filePath);
            }

            string apiKey =
                Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "GEMINI_API_KEY environment variable is not configured.");
            }

            int requestTimeoutSeconds = _timeoutSeconds;

             CancellationTokenSource timeoutCts =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(requestTimeoutSeconds));

             CancellationTokenSource linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutCts.Token);

            CancellationToken requestToken =
                linkedCts.Token;

            // Read selected file
            byte[] fileBytes;

            using (FileStream stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);

                    requestToken.ThrowIfCancellationRequested();

                    fileBytes = memoryStream.ToArray();
                }
            }

            // Convert file to Base64
            string base64Data =
                Convert.ToBase64String(fileBytes);

            // Determine MIME type
            string mimeType =
                GetMimeType(filePath);

            string fileName =
                System.IO.Path.GetFileName(filePath);

            // Gemini request
            var requestBody = new
            {
                systemInstruction = new
                {
                    parts = new[]
                    {
                new
                {
                    text =
                        "You are a helpful AI assistant for a student "
                        + "learning application. "
                        + "Analyze the attached file carefully and answer "
                        + "the user's question based on the file when relevant. "
                        + "Use clean plain text formatting. "
                        + "Do not use Markdown headings such as # or ##. "
                        + "Do not use Markdown bold or italic formatting "
                        + "such as **text** or *text*. "
                        + "Do not use decorative symbols or unnecessary emojis. "
                        + "For lists, use simple numbered lists or bullet points. "
                        + "For programming questions, preserve programming "
                        + "symbols and provide readable code."
                }
            }
                },

                contents = new[]
                {
            new
            {
                parts = new object[]
                {
                    new
                    {
                        text = string.IsNullOrWhiteSpace(userMessage)
                            ? $"Please analyze the attached file: {fileName}"
                            : userMessage
                    },

                    new
                    {
                        inlineData = new
                        {
                            mimeType = mimeType,
                            data = base64Data
                        }
                    }
                }
            }
        }
            };

            string json =
                JsonSerializer.Serialize(requestBody);

            string url =
                $"{Endpoint}?key={apiKey}";

             var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(
                    url,
                    content,
                    requestToken);
            }
            catch (OperationCanceledException)
                when (timeoutCts.IsCancellationRequested &&
                      !cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException(
                    $"The Gemini request timed out after " +
                    $"{_timeoutSeconds} seconds.");
            }
            catch (OperationCanceledException)
            {
                throw;
            }

            string responseJson =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new GeminiApiException(
                    response.StatusCode,
                    $"Gemini API request failed with status code " +
                    $"{(int)response.StatusCode} ({response.ReasonPhrase}).");
            }

             JsonDocument document =
                JsonDocument.Parse(responseJson);

            JsonElement root =
                document.RootElement;

            if (root.TryGetProperty(
                    "candidates",
                    out JsonElement candidates))
            {
                if (candidates.GetArrayLength() > 0)
                {
                    JsonElement candidate =
                        candidates[0];

                    if (candidate.TryGetProperty(
                            "content",
                            out JsonElement contentElement))
                    {
                        if (contentElement.TryGetProperty(
                                "parts",
                                out JsonElement parts))
                        {
                            foreach (JsonElement part
                                     in parts.EnumerateArray())
                            {
                                if (part.TryGetProperty(
                                        "text",
                                        out JsonElement text))
                                {
                                    string result =
                                        text.GetString();

                                    if (!string.IsNullOrWhiteSpace(result))
                                    {
                                        return CleanResponse(result);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException(
                "Gemini returned an empty or unexpected response.\n\n" +
                responseJson);
        }


      


        private static string GetMimeType(string filePath)
        {
            string extension =
                System.IO.Path.GetExtension(filePath)
                .ToLowerInvariant();

            switch (extension)
            {
                case ".png":
                    return "image/png";

                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";

                case ".webp":
                    return "image/webp";

                case ".gif":
                    return "image/gif";

                case ".bmp":
                    return "image/bmp";

                case ".pdf":
                    return "application/pdf";

                case ".txt":
                    return "text/plain";

                case ".csv":
                    return "text/csv";

                case ".json":
                    return "application/json";

                case ".xml":
                    return "application/xml";

                default:
                    return "application/octet-stream";
            }
        }

        private static string CleanResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                return string.Empty;
            }

            string text = response.Trim();

            // Remove Markdown code fence markers
            text = text.Replace("```csharp", "");
            text = text.Replace("```C#", "");
            text = text.Replace("```cs", "");
            text = text.Replace("```", "");

            // Remove Markdown bold
            text = text.Replace("**", "");

            // Remove Markdown underline/bold
            text = text.Replace("__", "");

            // Remove Markdown headings
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"(?m)^\s*#{1,6}\s*",
                "");

            // Convert Markdown bullets to normal bullet points
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"(?m)^\s*[*+-]\s+",
                "• ");

            // Remove Markdown horizontal separators
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"(?m)^\s*[-*_]{3,}\s*$",
                "");

            // Remove excessive blank lines
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"\n\s*\n\s*\n+",
                "\n\n");

            return text.Trim();
        }
    }
}
    

