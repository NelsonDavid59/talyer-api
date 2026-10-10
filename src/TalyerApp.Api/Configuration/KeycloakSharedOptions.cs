namespace TalyerApp.Api.Configuration;

public class KeycloakSharedOptions
{
    public const string SectionName = "Keycloak";

    public string? BaseUrl { get; set; }

    public string? Realm { get; set; }
}
