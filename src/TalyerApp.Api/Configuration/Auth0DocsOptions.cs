namespace TalyerApp.Api.Configuration;

public class Auth0DocsOptions
{
    public const string SectionName = "Auth0:Docs";

    public string? ScalarRoute { get; set; }

    public string? OAuthClientId { get; set; }

    public string[]? OAuthScopes { get; set; }
}
