# OAuth2 Authorization Code Flow Console Application

This repository contains a .NET console application that implements the OAuth2 Authorization Code Flow to obtain an access token and ID token from an authorization server.

## Features

- **Configuration-based setup**: The application reads settings like `AuthorizeEndpoint`, `TokenEndpoint`, and `ClientId` from the `appsettings.json` file.
- **Authorization Flow**: It initiates the OAuth2 Authorization Code Flow, redirecting users to the authorization server for login and consent.
- **Callback Listener**: It includes a simple HTTP listener that receives the authorization code and state via a redirect URL after authorization.
- **Token Exchange**: The app exchanges the received authorization code for tokens (access token and ID token) using the token endpoint.
- **Token Deserialization**: The application parses the token response into a usable format.

## Prerequisites

- .NET SDK installed
- Configuration for OAuth2 endpoints in `appsettings.json`:

  ```json
  {
    "AuthorizeEndpoint": "YOUR_AUTHORIZE_ENDPOINT",
    "TokenEndpoint": "YOUR_TOKEN_ENDPOINT",
    "ClientId": "YOUR_CLIENT_ID",
    "Scope": "YOUR_REQUESTED_SCOPES",
    "RedirectUrl": "YOUR_REDIRECT_URL"
  }
  ```

## Code Overview

- **Program.cs**: The entry point of the application, which sets up configurations and initiates the authorization process.

- **AuthorizeService.cs**: Contains methods for managing the OAuth2 authorization flow:

  - **`Initialize()`**:  
    Loads application settings from the `appsettings.json` file using `ConfigurationBuilder` and stores them in `_appSettings`. It throws an exception if settings are not found and calls the `Validate()` method to ensure the settings are correct. Additionally, it generates a new state string for use in processes of OAuth2.

  - **`InitiateAuthorizationCodeFlow()`**:  
    Starts the OAuth2 Authorization Code Flow by launching the browser and directing the user to the authorization server’s endpoint. It sends required parameters like `response_type`, `client_id`, `scope`, and `redirect_uri`.

  - **`InitiateCallbackListner()`**:  
    Starts a local HTTP listener that waits for the authorization code and state to be returned by the authorization server after user consent. Once received, it processes the response and closes the listener.

  - **`InitiateCodeExchange(string authorizationCode, string state)`**:  
    Exchanges the authorization code for tokens by making a POST request to the token endpoint. The method sends the `authorization_code`, along with client credentials and the redirect URI to request an access token and ID token.

  - **`DeserializeCodeExchangeResult(string codeExchangeResult)`**:  
    Deserializes the token response (which includes the access token and ID token) from the server into a `TokenResponse` object for use in the application.

  - **`GenerateState()`**:  
    Generates a cryptographically secure state parameter for OAuth2 requests. This state is used to prevent CSRF (Cross-Site Request Forgery) attacks.

  - **`LaunchBrowser(string url, Dictionary<string, string> requestParams)`**:  
    Opens the default browser on the user’s machine with the constructed authorization URL, passing the necessary request parameters.

  - **`IsStateCorrelated(string state)`**:  
    Compares the provided `state` with the predefined `_state`. Returns `true` if they match, ensuring the state is valid in contexts like OAuth2 to prevent CSRF attacks.

- **AppSettings.cs**: Defines the application settings that are read from the `appsettings.json` file. These include endpoints, client credentials, and redirect URLs.

- **TokenResponse.cs**: A class for deserializing the token response received from the token endpoint. This includes fields like `IdToken` and `AccessToken`.
