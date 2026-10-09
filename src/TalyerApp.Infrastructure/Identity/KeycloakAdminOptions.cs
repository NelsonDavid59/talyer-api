namespace TalyerApp.Infrastructure.Identity;

public class KeycloakAdminOptions
{
    public const string SectionName = "Keycloak:Admin";

    public string BaseUrl { get; set; } = "http://localhost:8080";

    public string Realm { get; set; } = "talyer-realm";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}
