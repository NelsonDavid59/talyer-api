namespace TalyerApp.Api.Configuration;

public static class Auth0UrlBuilder
{
    public static string GetAuthority(Auth0ApiOptions api, Auth0SharedOptions shared)
    {
        if (!string.IsNullOrWhiteSpace(api.Authority))
        {
            return api.Authority.TrimEnd('/');
        }

        var baseUrl = Auth0ConnectionResolver.ResolveBaseUrl(api.BaseUrl, shared.BaseUrl);
        var realm = Auth0ConnectionResolver.ResolveRealm(api.Realm, shared.Realm);

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(realm))
        {
            throw new InvalidOperationException(
                "Cannot resolve Auth0 authority. Set Auth0:Api:Authority, or Auth0:BaseUrl and Auth0:Realm, or Auth0:Api:BaseUrl and Auth0:Api:Realm.");
        }

        return $"{baseUrl.TrimEnd('/')}/realms/{realm.Trim()}";
    }

    public static string GetAuthorizationEndpoint(Auth0ApiOptions api, Auth0SharedOptions shared) =>
        $"{GetAuthority(api, shared)}/protocol/openid-connect/auth";

    public static string GetTokenEndpoint(Auth0ApiOptions api, Auth0SharedOptions shared) =>
        $"{GetAuthority(api, shared)}/protocol/openid-connect/token";
}
