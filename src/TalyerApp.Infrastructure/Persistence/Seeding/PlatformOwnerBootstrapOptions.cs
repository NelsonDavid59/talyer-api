namespace TalyerApp.Infrastructure.Persistence.Seeding;

public class PlatformOwnerBootstrapOptions
{
    public const string SectionName = "Bootstrap:PlatformOwner";

    public string Provider { get; set; } = string.Empty;

    public string ProviderUserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
