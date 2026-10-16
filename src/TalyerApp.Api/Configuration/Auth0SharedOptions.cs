namespace TalyerApp.Api.Configuration;

public class Auth0SharedOptions
{
    public const string SectionName = "Auth0";

    public string? BaseUrl { get; set; }

    public string? Realm { get; set; }
}
