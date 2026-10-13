namespace TalyerApp.Api.Configuration;

public static class KeycloakConnectionResolver
{
    public static string? ResolveBaseUrl(string? sectionBaseUrl, string? sharedBaseUrl) =>
        string.IsNullOrWhiteSpace(sectionBaseUrl) ? sharedBaseUrl : sectionBaseUrl;

    public static string? ResolveRealm(string? sectionRealm, string? sharedRealm) =>
        string.IsNullOrWhiteSpace(sectionRealm) ? sharedRealm : sectionRealm;

    public static string GetNormalizedBaseUrl(string? sectionBaseUrl, string? sharedBaseUrl)
    {
        var baseUrl = ResolveBaseUrl(sectionBaseUrl, sharedBaseUrl);
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "Keycloak BaseUrl is not configured. Set Keycloak:BaseUrl or a section-specific BaseUrl.");
        }

        return baseUrl.TrimEnd('/');
    }
}
