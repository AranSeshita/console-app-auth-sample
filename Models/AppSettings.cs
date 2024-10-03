public class AppSettings
{
    //Authority configurations
    public string AuthorizeEndpoint { get; set; }
    public string TokenEndpoint { get; set; }

    //Client configurations
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
    public string CallBackUrl { get; set; }
    public string RedirectUrl { get; set; }
    public void Validate()
    {
        ValidateProperty(AuthorizeEndpoint, nameof(AuthorizeEndpoint));
        ValidateProperty(TokenEndpoint, nameof(TokenEndpoint));
        ValidateProperty(ClientId, nameof(ClientId));
        ValidateProperty(ClientSecret, nameof(ClientSecret));
        ValidateProperty(Scope, nameof(Scope));
        ValidateProperty(CallBackUrl, nameof(CallBackUrl));
        ValidateProperty(RedirectUrl, nameof(RedirectUrl));
    }

    private void ValidateProperty(string propertyValue, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyValue))
        {
            throw new Exception($"{propertyName} is missing in appsettings.json");
        }
    }
}