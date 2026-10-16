namespace TalyerApp.Api.Configuration;

public class Auth0ApiOptions
{
    public const string SectionName = "Auth0:Api";

    public string? BaseUrl { get; set; }

    public string? Realm { get; set; }

    /// <summary>
    /// JWT authority. When empty, derived from <see cref="BaseUrl"/> and <see cref="Realm"/>.
    /// </summary>
    public string? Authority { get; set; }

    public string? Audience { get; set; }

    public bool? RequireHttpsMetadata { get; set; }
}
