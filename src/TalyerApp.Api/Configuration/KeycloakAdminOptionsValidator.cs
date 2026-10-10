using Microsoft.Extensions.Options;
using TalyerApp.Infrastructure.Identity;

namespace TalyerApp.Api.Configuration;

public sealed class KeycloakAdminOptionsValidator : IValidateOptions<KeycloakAdminOptions>
{
    private readonly KeycloakSharedOptions _shared;

    public KeycloakAdminOptionsValidator(IOptions<KeycloakSharedOptions> shared)
    {
        _shared = shared.Value;
    }

    public ValidateOptionsResult Validate(string? name, KeycloakAdminOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ClientId))
        {
            failures.Add($"{KeycloakAdminOptions.SectionName}:ClientId is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ClientSecret))
        {
            failures.Add($"{KeycloakAdminOptions.SectionName}:ClientSecret is required.");
        }

        if (string.IsNullOrWhiteSpace(KeycloakConnectionResolver.ResolveBaseUrl(options.BaseUrl, _shared.BaseUrl)))
        {
            failures.Add(
                $"{KeycloakAdminOptions.SectionName}:BaseUrl is required (set Keycloak:Admin:BaseUrl or Keycloak:BaseUrl).");
        }

        if (string.IsNullOrWhiteSpace(KeycloakConnectionResolver.ResolveRealm(options.Realm, _shared.Realm)))
        {
            failures.Add(
                $"{KeycloakAdminOptions.SectionName}:Realm is required (set Keycloak:Admin:Realm or Keycloak:Realm).");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
