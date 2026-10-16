namespace TalyerApp.Infrastructure.Identity;

public class Auth0AdminOptions
{
    public const string SectionName = "Auth0:Admin";

    public string? BaseUrl { get; set; }

    public string? Realm { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }
}
