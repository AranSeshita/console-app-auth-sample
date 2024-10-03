using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

//Initialize configurations
var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();
var appSettings = configuration.Get<AppSettings>();
appSettings?.Validate();

//Initialize AuthorizeService
AuthorizeService.Initialize(appSettings);

//Initiate the authorization request
Console.WriteLine($"Initiating Authorization Request to {appSettings.AuthorizeEndpoint}...");
await AuthorizeService.InitiateAuthorizationCodeFlow();

//Initiate the callback listener
var callbackResult = await AuthorizeService.InitiateCallbackListner();

//Initiate code exchange request for tokens
var codeExchangeResult = await AuthorizeService.InitiateCodeExchange(callbackResult.authorizationCode, callbackResult.state);

//Deserialize code exchange request result
var tokenResponse = await AuthorizeService.DeserializeCodeExchangeResult(codeExchangeResult);
Console.WriteLine("id_token: " + tokenResponse.IdToken);
Console.WriteLine("access_token: " + tokenResponse.AccessToken);
Console.ReadLine();