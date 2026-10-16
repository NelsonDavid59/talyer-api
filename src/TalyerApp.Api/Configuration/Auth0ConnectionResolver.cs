namespace TalyerApp.Api.Configuration;

public static class Auth0ConnectionResolver
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
                "Auth0 BaseUrl is not configured. Set Auth0:BaseUrl or a section-specific BaseUrl.");
        }

        return baseUrl.TrimEnd('/');
    }
}
