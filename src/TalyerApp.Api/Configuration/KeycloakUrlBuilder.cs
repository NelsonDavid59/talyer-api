namespace TalyerApp.Api.Configuration;

public static class KeycloakUrlBuilder
{
    public static string GetAuthority(KeycloakApiOptions api, KeycloakSharedOptions shared)
    {
        if (!string.IsNullOrWhiteSpace(api.Authority))
        {
            return api.Authority.TrimEnd('/');
        }

        var baseUrl = KeycloakConnectionResolver.ResolveBaseUrl(api.BaseUrl, shared.BaseUrl);
        var realm = KeycloakConnectionResolver.ResolveRealm(api.Realm, shared.Realm);

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(realm))
        {
            throw new InvalidOperationException(
                "Cannot resolve Keycloak authority. Set Keycloak:Api:Authority, or Keycloak:BaseUrl and Keycloak:Realm, or Keycloak:Api:BaseUrl and Keycloak:Api:Realm.");
        }

        return $"{baseUrl.TrimEnd('/')}/realms/{realm.Trim()}";
    }

    public static string GetAuthorizationEndpoint(KeycloakApiOptions api, KeycloakSharedOptions shared) =>
        $"{GetAuthority(api, shared)}/protocol/openid-connect/auth";

    public static string GetTokenEndpoint(KeycloakApiOptions api, KeycloakSharedOptions shared) =>
        $"{GetAuthority(api, shared)}/protocol/openid-connect/token";
}
