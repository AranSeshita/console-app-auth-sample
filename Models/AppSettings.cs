public class AppSettings
{
    //Authority configurations
    public string AuthorizeEndpoint { get; set; }
    public string TokenEndpoint { get; set; }

    //Client configurations
    public string ClientId { get; set; }
    public string Scope { get; set; }
    public string RedirectUrl { get; set; }
    public string CodeChallengeMethod { get; set; }
    public void Validate()
    {
        ValidateProperty(AuthorizeEndpoint, nameof(AuthorizeEndpoint));
        ValidateProperty(TokenEndpoint, nameof(TokenEndpoint));
        ValidateProperty(ClientId, nameof(ClientId));
        ValidateProperty(Scope, nameof(Scope));
        ValidateProperty(RedirectUrl, nameof(RedirectUrl));
        ValidateProperty(CodeChallengeMethod, nameof(CodeChallengeMethod));
    }

    private void ValidateProperty(string propertyValue, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyValue))
        {
            throw new Exception($"{propertyName} is missing in appsettings.json");
        }
    }
}