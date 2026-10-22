using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public class GoogleOAuthService
    {
        private const string AuthorizationEndpoint =
            "https://accounts.google.com/o/oauth2/v2/auth";

        private const string TokenEndpoint =
            "https://oauth2.googleapis.com/token";

        private const string RedirectUri =
            "http://127.0.0.1:5000/";

        private HttpListener _httpListener;

        private string _codeVerifier;

        public async Task<GoogleUserInfo> StartGoogleLoginAsync()
        {
            string clientId =
                GoogleOAuthConfiguration.ClientId;

            // Generate PKCE code verifier
            _codeVerifier =
                GenerateCodeVerifier();

            // Generate PKCE code challenge
            string codeChallenge =
                GenerateCodeChallenge(_codeVerifier);

            string scope =
                Uri.EscapeDataString(
                    "openid email profile");

            string authorizationUrl =
                AuthorizationEndpoint +
                "?client_id=" +
                Uri.EscapeDataString(clientId) +
                "&redirect_uri=" +
                Uri.EscapeDataString(RedirectUri) +
                "&response_type=code" +
                "&scope=" +
                scope +
                "&code_challenge=" +
                Uri.EscapeDataString(codeChallenge) +
                "&code_challenge_method=S256" +
                "&prompt=select_account";

            StartCallbackListener();

            Process.Start(
                new ProcessStartInfo
                {
                    FileName = authorizationUrl,
                    UseShellExecute = true
                });

            return await WaitForGoogleLoginAsync();
        }

        private TaskCompletionSource<GoogleUserInfo>
            _googleLoginCompletion =
                new TaskCompletionSource<GoogleUserInfo>();

        private void StartCallbackListener()
        {
            _httpListener =
                new HttpListener();

            _httpListener.Prefixes.Add(
                RedirectUri);

            _httpListener.Start();

            Task.Run(async () =>
            {
                HttpListenerContext context = null;

                try
                {
                    context =
                        await _httpListener.GetContextAsync();

                    string code =
                        context.Request.QueryString["code"];

                    string error =
                        context.Request.QueryString["error"];

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        SendBrowserResponse(
                            context,
                            "Google Login Failed",
                            "Google returned an OAuth error: " +
                            error);

                        _httpListener.Stop();

                        _googleLoginCompletion.TrySetException(
                            new Exception(
                                "Google returned an OAuth error: " +
                                error));

                        return;
                    }

                    if (string.IsNullOrWhiteSpace(code))
                    {
                        SendBrowserResponse(
                            context,
                            "Google Login Failed",
                            "Authorization code was not received.");

                        _httpListener.Stop();

                        _googleLoginCompletion.TrySetException(
                            new Exception(
                                "Authorization code was not received."));

                        return;
                    }

                    GoogleUserInfo googleUser =
                        await ExchangeCodeForTokenAsync(code);


                    SendGoogleLoginSuccessResponse(context);


                    _httpListener.Stop();

                    _googleLoginCompletion.TrySetResult(
                        googleUser);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(
                        "Google OAuth Listener Error: " +
                        ex);

                    try
                    {
                        if (context != null)
                        {
                            SendBrowserResponse(
                                context,
                                "Google Login Failed",
                                ex.Message);
                        }
                    }
                    catch (Exception responseException)
                    {
                        Debug.WriteLine(
                            "Google OAuth Browser Response Error: " +
                            responseException);
                    }

                    try
                    {
                        if (_httpListener != null &&
                            _httpListener.IsListening)
                        {
                            _httpListener.Stop();
                        }
                    }
                    catch
                    {
                        // Ignore listener cleanup errors
                    }

                    _googleLoginCompletion.TrySetException(ex);
                }
            });
        }

        private async Task<GoogleUserInfo>
            WaitForGoogleLoginAsync()
        {
            return await _googleLoginCompletion.Task;
        }

        private async Task<GoogleUserInfo>
            ExchangeCodeForTokenAsync(
                string code)
        {
            string clientId =
                GoogleOAuthConfiguration.ClientId;

            using (HttpClient client =
                   new HttpClient())
            {
                var values =
                    new Dictionary<string, string>
                    {
                        {
                            "code",
                            code
                        },
                        {
                            "client_id",
                            clientId
                        },
                        {
                            "client_secret",
                            GoogleOAuthConfiguration.ClientSecret
                        },
                        {
                            "redirect_uri",
                            RedirectUri
                        },
                        {
                            "grant_type",
                            "authorization_code"
                        },
                        {
                            "code_verifier",
                            _codeVerifier
                        }
                    };

                using (var content =
                       new FormUrlEncodedContent(values))
                {
                    HttpResponseMessage response =
                        await client.PostAsync(
                            TokenEndpoint,
                            content);

                    string responseBody =
                        await response.Content
                            .ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            "Google token request failed.\n\n" +
                            responseBody);
                    }

                    JObject tokenResponse =
                        JObject.Parse(responseBody);

                    string accessToken =
                        tokenResponse["access_token"]
                            ?.ToString();

                    string idToken =
                        tokenResponse["id_token"]
                            ?.ToString();

                    if (string.IsNullOrWhiteSpace(
                        accessToken))
                    {
                        throw new Exception(
                            "Google did not return an access token.");
                    }

                    Debug.WriteLine(
                        "Google access token received: YES");

                    Debug.WriteLine(
                        "Google ID token received: " +
                        (
                            !string.IsNullOrWhiteSpace(idToken)
                                ? "YES"
                                : "NO"
                        ));

                    GoogleUserInfo googleUser =
                        await GetGoogleUserInfoAsync(
                            accessToken);

                    return googleUser;
                }
            }
        }

        private async Task<GoogleUserInfo>
            GetGoogleUserInfoAsync(
                string accessToken)
        {
            const string userInfoEndpoint =
                "https://openidconnect.googleapis.com/v1/userinfo";

            using (HttpClient client =
                   new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers
                        .AuthenticationHeaderValue(
                            "Bearer",
                            accessToken);

                HttpResponseMessage response =
                    await client.GetAsync(
                        userInfoEndpoint);

                string responseBody =
                    await response.Content
                        .ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        "Google UserInfo request failed.\n\n" +
                        responseBody);
                }

                JObject userInfo =
                    JObject.Parse(responseBody);

                string googleUserId =
                    userInfo["sub"]?.ToString();

                string email =
                    userInfo["email"]?.ToString();

                string name =
                    userInfo["name"]?.ToString();

                string givenName =
                    userInfo["given_name"]?.ToString();

                string familyName =
                    userInfo["family_name"]?.ToString();

                string picture =
                    userInfo["picture"]?.ToString();

                bool emailVerified =
                    userInfo["email_verified"]
                        ?.ToObject<bool>()
                    ?? false;

                if (string.IsNullOrWhiteSpace(
                    googleUserId))
                {
                    throw new Exception(
                        "Google UserInfo did not return a user ID.");
                }

                Debug.WriteLine(
                    "===== GOOGLE USER INFO =====");

                Debug.WriteLine(
                    "Google User ID: " +
                    googleUserId);

                Debug.WriteLine(
                    "Google Email: " +
                    email);

                Debug.WriteLine(
                    "Google Name: " +
                    name);

                Debug.WriteLine(
                    "Google Given Name: " +
                    givenName);

                Debug.WriteLine(
                    "Google Family Name: " +
                    familyName);

                Debug.WriteLine(
                    "Google Email Verified: " +
                    emailVerified);

                Debug.WriteLine(
                    "Google Picture Available: " +
                    (
                        !string.IsNullOrWhiteSpace(picture)
                            ? "YES"
                            : "NO"
                    ));

                Debug.WriteLine(
                    "============================");

                return new GoogleUserInfo
                {
                    GoogleUserId = googleUserId,
                    Email = email,
                    Name = name,
                    GivenName = givenName,
                    FamilyName = familyName,
                    Picture = picture,
                    EmailVerified = emailVerified
                };
            }
        }


        private void SendGoogleLoginSuccessResponse(
    HttpListenerContext context)
        {
            string html = @"
<!DOCTYPE html>

<html>

<head>

    <meta charset=""UTF-8"">

    <meta name=""viewport""
          content=""width=device-width, initial-scale=1.0"">

    <title>Google Login Successful</title>

    <style>

        body
        {
            margin: 0;

            padding: 0;

            font-family:
                Arial,
                Helvetica,
                sans-serif;

            background:
                #f5f7fb;

            display: flex;

            justify-content: center;

            align-items: center;

            min-height: 100vh;
        }

        .container
        {
            width: 450px;

            max-width: 90%;

            background: white;

            padding: 40px;

            border-radius: 16px;

            text-align: center;

            box-shadow:
                0 8px 30px
                rgba(0, 0, 0, 0.10);
        }

        .success
        {
            font-size: 28px;

            font-weight: bold;

            margin-bottom: 20px;

            color: #222;
        }

        .message
        {
            font-size: 16px;

            color: #555;

            line-height: 1.6;

            margin-bottom: 25px;
        }

        .countdown
        {
            font-size: 20px;

            font-weight: bold;

            color: #333;

            margin-bottom: 25px;
        }

        .button
        {
            display: inline-block;

            padding: 12px 28px;

            border: none;

            border-radius: 8px;

            background: #1a73e8;

            color: white;

            font-size: 16px;

            font-weight: bold;

            cursor: pointer;

            text-decoration: none;
        }

        .button:hover
        {
            background: #1557b0;
        }

    </style>

</head>


<body>

    <div class=""container"">

        <div class=""success"">
            Google Login Successful
        </div>

        <div class=""message"">

            Your Google account has been
            authenticated successfully.

            <br><br>

            ChatAI will open automatically.

        </div>

        <div
            id=""countdown""
            class=""countdown"">

            Opening ChatAI in 3 seconds...

        </div>

        <button
            class=""button""
            onclick=""openChatAI()"">

            Open ChatAI

        </button>

    </div>


    <script>

        function openChatAI()
        {
            window.location.href =
                'openchatai://login';
        }


        let seconds = 3;


        const timer =
            setInterval(
                function()
                {
                    seconds--;

                    const countdown =
                        document.getElementById(
                            'countdown');

                    if (seconds > 0)
                    {
                        countdown.innerText =
                            'Opening ChatAI in '
                            + seconds
                            + ' seconds...';
                    }
                    else
                    {
                        clearInterval(timer);

                        countdown.innerText =
                            'Opening ChatAI...';

                        openChatAI();
                    }
                },
                1000);

    </script>

</body>

</html>";

            byte[] buffer =
                Encoding.UTF8.GetBytes(html);

            context.Response.ContentType =
                "text/html; charset=utf-8";

            context.Response.ContentEncoding =
                Encoding.UTF8;

            context.Response.ContentLength64 =
                buffer.Length;

            using (Stream output =
                   context.Response.OutputStream)
            {
                output.Write(
                    buffer,
                    0,
                    buffer.Length);

                output.Flush();
            }
        }




        private static string GenerateCodeVerifier()
        {
            byte[] bytes =
                new byte[32];

            using (RandomNumberGenerator rng =
                   RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return Base64UrlEncode(bytes);
        }

        private static string GenerateCodeChallenge(
            string codeVerifier)
        {
            byte[] bytes =
                Encoding.ASCII.GetBytes(
                    codeVerifier);

            byte[] hash;

            using (SHA256 sha256 =
                   SHA256.Create())
            {
                hash =
                    sha256.ComputeHash(bytes);
            }

            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(
            byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static void SendBrowserResponse(
            HttpListenerContext context,
            string title,
            string message)
        {
            string responseHtml =
                "<html>" +
                "<head>" +
                "<title>" +
                title +
                "</title>" +
                "</head>" +
                "<body>" +
                "<h2>" +
                title +
                "</h2>" +
                "<p>" +
                message +
                "</p>" +
                "</body>" +
                "</html>";

            byte[] buffer =
                Encoding.UTF8.GetBytes(
                    responseHtml);

            context.Response.ContentType =
                "text/html";

            context.Response.ContentLength64 =
                buffer.Length;

            context.Response.OutputStream.Write(
                buffer,
                0,
                buffer.Length);

            context.Response.OutputStream.Close();
        }

        public class GoogleUserInfo
        {
            public string GoogleUserId { get; set; }

            public string Email { get; set; }

            public string Name { get; set; }

            public string GivenName { get; set; }

            public string FamilyName { get; set; }

            public string Picture { get; set; }

            public bool EmailVerified { get; set; }
        }
    }
}