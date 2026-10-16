namespace TalyerApp.Api.Configuration;

public class TalyerSessionOptions
{
    public const string SectionName = "Talyer:Session";

    public string? Issuer { get; set; }

    public string? Audience { get; set; }

    public string? SigningKey { get; set; }

    public int LifetimeMinutes { get; set; } = 480;
}
