namespace TalyerApp.Infrastructure.Persistence.Seeding;

public class PlatformAdminBootstrapOptions
{
    public const string SectionName = "Bootstrap:PlatformAdmin";

    public string Provider { get; set; } = "keycloak";

    public string ProviderUserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
