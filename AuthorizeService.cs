using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public static class AuthorizeService
{
    private static AppSettings _appSettings;
    private static string _state;
    private static string _codeVerifier;
    public static void Initialize()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _appSettings = configuration.Get<AppSettings>() ?? throw new Exception("AppSettings not found in appsettings.json.");
        _appSettings?.Validate();
        _state = GenerateState();
        _codeVerifier = GenerateCodeVerifier();
    }
    public static async Task InitiateAuthorizationCodeFlow()
    {
        var requestParams = new Dictionary<string, string>();
        requestParams["response_type"] = "code";
        requestParams["client_id"] = _appSettings.ClientId;
        requestParams["redirect_uri"] = _appSettings.RedirectUrl;
        requestParams["scope"] = _appSettings.Scope;
        requestParams["code_challenge_method"] = _appSettings.CodeChallengeMethod;
        requestParams["code_challenge"] = GenerateCodeChallenge();
        requestParams["state"] = _state;

        Console.WriteLine($"Initiating Authorization Request to {_appSettings.AuthorizeEndpoint}...");
        await LaunchBrowser(_appSettings.AuthorizeEndpoint, requestParams);
    }

    public static async Task<(string authorizationCode, string state)> InitiateCallbackListner()
    {
        HttpListener listener = new HttpListener();
        var callbackUrl = _appSettings.RedirectUrl.EndsWith("/") ? _appSettings.RedirectUrl : _appSettings.RedirectUrl + "/";
        listener.Prefixes.Add(callbackUrl);
        listener.Start();
        Console.WriteLine($"Listning on {callbackUrl}...");

        // The GetContext method blocks while waiting for a request.
        HttpListenerContext context = await listener.GetContextAsync();
        HttpListenerRequest request = context.Request;

        NameValueCollection queryParams = request.QueryString;
        string authorizationCode = queryParams["code"] ?? throw new Exception("Authorization code not found in query parameters.");
        string state = queryParams["state"] ?? throw new Exception("State not found in query parameters.");
        if (!IsStateCorrelated(state)) throw new Exception("State is not correlated.");
        Console.WriteLine("Successfully received authorization code and state.");

        HttpListenerResponse response = context.Response;
        string responseString = "<html><body>Now you can close the window.</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        System.IO.Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
        listener.Stop();

        return (authorizationCode, state);
    }

    public static async Task<string> InitiateCodeExchange(string authorizationCode, string state)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authorizationCode },
                { "redirect_uri", _appSettings.RedirectUrl },
                { "client_id", _appSettings.ClientId },
                { "code_verifier", _codeVerifier },
            };

                var content = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync(_appSettings.TokenEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return errorContent;
                }
            }
        }
        catch (System.Exception e)
        {
            throw new Exception("Error occured while exchanging code for token.", e);
        }
    }

    public static async Task<TokenResponse> DeserializeCodeExchangeResult(string codeExchangeResult)
    {
        try
        {
            return await Task.Run(() => JsonSerializer.Deserialize<TokenResponse>(codeExchangeResult));
        }
        catch (System.Exception e)
        {
            throw new Exception("Error occured while deserializing token response.", e);
        }
    }

    private static string GenerateState()
    {
        string base64State = GenerateRandomBase64();
        return Base64UrlEncode(base64State);
    }
    private static string GenerateCodeVerifier()
    {
        return GenerateRandomBase64();
    }

    private static string GenerateCodeChallenge()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(_codeVerifier);
            byte[] hash = sha256.ComputeHash(bytes);
            string codeChallenge = Convert.ToBase64String(hash);
            return Base64UrlEncode(codeChallenge);
        }
    }

    private static string Base64UrlEncode(string tectToEncode)
    {
        return tectToEncode.Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static string GenerateRandomBase64()
    {
        byte[] randomBytes = new byte[32];
        RandomNumberGenerator.Fill(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static async Task LaunchBrowser(string url, Dictionary<string, string> requestParams)
    {
        var strParams = await new FormUrlEncodedContent(requestParams).ReadAsStringAsync();
        url += $"?{strParams}";
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private static bool IsStateCorrelated(string state)
    {
        return state == _state;
    }
}