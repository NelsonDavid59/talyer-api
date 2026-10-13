using Microsoft.Extensions.Options;

namespace TalyerApp.Api.Configuration;

public sealed class KeycloakApiOptionsValidator : IValidateOptions<KeycloakApiOptions>
{
    private readonly KeycloakSharedOptions _shared;

    public KeycloakApiOptionsValidator(IOptions<KeycloakSharedOptions> shared)
    {
        _shared = shared.Value;
    }

    public ValidateOptionsResult Validate(string? name, KeycloakApiOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add($"{KeycloakApiOptions.SectionName}:Audience is required.");
        }

        if (options.RequireHttpsMetadata is null)
        {
            failures.Add($"{KeycloakApiOptions.SectionName}:RequireHttpsMetadata must be set explicitly (true or false).");
        }

        var hasAuthority = !string.IsNullOrWhiteSpace(options.Authority);
        var effectiveBaseUrl = KeycloakConnectionResolver.ResolveBaseUrl(options.BaseUrl, _shared.BaseUrl);
        var effectiveRealm = KeycloakConnectionResolver.ResolveRealm(options.Realm, _shared.Realm);
        var hasBaseUrlAndRealm =
            !string.IsNullOrWhiteSpace(effectiveBaseUrl) &&
            !string.IsNullOrWhiteSpace(effectiveRealm);

        if (!hasAuthority && !hasBaseUrlAndRealm)
        {
            failures.Add(
                $"{KeycloakApiOptions.SectionName}: set Authority, or configure Keycloak:BaseUrl and Keycloak:Realm, or Keycloak:Api:BaseUrl and Keycloak:Api:Realm.");
        }

        if (!hasAuthority)
        {
            if (!string.IsNullOrWhiteSpace(options.BaseUrl) && string.IsNullOrWhiteSpace(options.Realm) &&
                string.IsNullOrWhiteSpace(_shared.Realm))
            {
                failures.Add($"{KeycloakApiOptions.SectionName}:Realm is required when Api BaseUrl is set without Keycloak:Realm.");
            }

            if (string.IsNullOrWhiteSpace(options.BaseUrl) && !string.IsNullOrWhiteSpace(options.Realm) &&
                string.IsNullOrWhiteSpace(_shared.BaseUrl))
            {
                failures.Add($"{KeycloakApiOptions.SectionName}:BaseUrl is required when Api Realm is set without Keycloak:BaseUrl.");
            }
        }

        try
        {
            if (failures.Count == 0)
            {
                KeycloakUrlBuilder.GetAuthority(options, _shared);
            }
        }
        catch (InvalidOperationException ex)
        {
            failures.Add(ex.Message);
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
