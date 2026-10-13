namespace TalyerApp.Api.Configuration;

public class KeycloakDocsOptions
{
    public const string SectionName = "Keycloak:Docs";

    public string? ScalarRoute { get; set; }

    public string? OAuthClientId { get; set; }

    public string[]? OAuthScopes { get; set; }
}
