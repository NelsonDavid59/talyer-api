namespace TalyerApp.Infrastructure.Identity;

public class KeycloakAdminOptions
{
    public const string SectionName = "Keycloak:Admin";

    public string? BaseUrl { get; set; }

    public string? Realm { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }
}
