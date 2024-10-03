Console.WriteLine("Hello, World!");

//Initialize configurations
AuthorizeService.Initialize();

//Initiate the authorization request
await AuthorizeService.InitiateAuthorizationCodeFlow();

//Initiate the callback listener
var callbackResult = await AuthorizeService.InitiateCallbackListner();

//Initiate the code exchange request for tokens
var codeExchangeResult = await AuthorizeService.InitiateCodeExchange(callbackResult.authorizationCode, callbackResult.state);

//Deserialize the result of code exchange request
var tokenResponse = await AuthorizeService.DeserializeCodeExchangeResult(codeExchangeResult);
Console.WriteLine("id_token: " + tokenResponse.IdToken);
Console.WriteLine("access_token: " + tokenResponse.AccessToken);

Console.ReadLine();