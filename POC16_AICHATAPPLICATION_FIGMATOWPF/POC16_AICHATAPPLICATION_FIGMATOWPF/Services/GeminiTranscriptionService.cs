using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public class GeminiTranscriptionService
    {
        private readonly HttpClient _httpClient;

        private const string Model =
            "gemini-3.1-flash-lite";

        private const string Endpoint =
            "https://generativelanguage.googleapis.com/v1beta/models/"
            + Model
            + ":generateContent";

        public GeminiTranscriptionService()
        {
            _httpClient = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public async Task<string> TranscribeAsync(
            byte[] audioData,
            CancellationToken cancellationToken = default)
        {
            if (audioData == null ||
                audioData.Length == 0)
            {
                throw new ArgumentException(
                    "Audio data cannot be empty.",
                    nameof(audioData));
            }

            // ==========================================
            // GET GEMINI API KEY
            // ==========================================

            string apiKey =
                Environment.GetEnvironmentVariable(
                    "GEMINI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "GEMINI_API_KEY environment variable " +
                    "is not configured.");
            }

            // ==========================================
            // CONVERT AUDIO TO BASE64
            // ==========================================

            string base64Audio =
                Convert.ToBase64String(audioData);

            // ==========================================
            // GEMINI REQUEST
            // ==========================================

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",

                        parts = new object[]
                        {
                            new
                            {
                                inlineData = new
                                {
                                    mimeType = "audio/wav",
                                    data = base64Audio
                                }
                            },

                            new
                            {
                                text =
                                    "Transcribe the spoken audio. " +
                                    "Return only the exact spoken text. " +
                                    "Do not summarize. " +
                                    "Do not explain. " +
                                    "Do not add quotation marks."
                            }
                        }
                    }
                }
            };

            // ==========================================
            // SERIALIZE REQUEST
            // ==========================================

            string json =
                JsonSerializer.Serialize(requestBody);

            string url =
                $"{Endpoint}?key={apiKey}";

             var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            // ==========================================
            // SEND REQUEST
            // ==========================================

            HttpResponseMessage response =
                await _httpClient.PostAsync(
                    url,
                    content,
                    cancellationToken);

            string responseJson =
                await response.Content
                    .ReadAsStringAsync();

            // ==========================================
            // CHECK RESPONSE
            // ==========================================

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Gemini transcription request failed.\n\n" +
                    $"Status Code: {(int)response.StatusCode}\n" +
                    $"Response: {responseJson}");
            }

            // ==========================================
            // PARSE RESPONSE
            // ==========================================

             JsonDocument document =
                JsonDocument.Parse(responseJson);

            JsonElement root =
                document.RootElement;

            if (!root.TryGetProperty(
                    "candidates",
                    out JsonElement candidates))
            {
                throw new Exception(
                    "Gemini transcription response " +
                    "does not contain candidates.");
            }

            if (candidates.GetArrayLength() == 0)
            {
                throw new Exception(
                    "Gemini returned no transcription result.");
            }

            JsonElement candidate =
                candidates[0];

            if (!candidate.TryGetProperty(
                    "content",
                    out JsonElement contentElement))
            {
                throw new Exception(
                    "Gemini response does not contain content.");
            }

            if (!contentElement.TryGetProperty(
                    "parts",
                    out JsonElement parts))
            {
                throw new Exception(
                    "Gemini response does not contain parts.");
            }

            // ==========================================
            // EXTRACT TRANSCRIPTION
            // ==========================================

            StringBuilder transcription =
                new StringBuilder();

            foreach (JsonElement part
                     in parts.EnumerateArray())
            {
                if (part.TryGetProperty(
                        "text",
                        out JsonElement textElement))
                {
                    string text =
                        textElement.GetString();

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        transcription.Append(text);
                    }
                }
            }

            string result =
                transcription.ToString().Trim();

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new Exception(
                    "Gemini returned an empty transcription.");
            }

            return result;
        }
    }
}