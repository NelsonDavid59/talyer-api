using Microsoft.Extensions.Options;

namespace TalyerApp.Api.Configuration;

public sealed class KeycloakDocsOptionsValidator : IValidateOptions<KeycloakDocsOptions>
{
    public ValidateOptionsResult Validate(string? name, KeycloakDocsOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ScalarRoute))
        {
            failures.Add($"{KeycloakDocsOptions.SectionName}:ScalarRoute is required.");
        }
        else if (!options.ScalarRoute.StartsWith("/", StringComparison.Ordinal))
        {
            failures.Add($"{KeycloakDocsOptions.SectionName}:ScalarRoute must start with '/'.");
        }

        if (string.IsNullOrWhiteSpace(options.OAuthClientId))
        {
            failures.Add($"{KeycloakDocsOptions.SectionName}:OAuthClientId is required.");
        }

        if (options.OAuthScopes is null || options.OAuthScopes.Length == 0)
        {
            failures.Add($"{KeycloakDocsOptions.SectionName}:OAuthScopes must contain at least one scope.");
        }
        else if (options.OAuthScopes.Any(string.IsNullOrWhiteSpace))
        {
            failures.Add($"{KeycloakDocsOptions.SectionName}:OAuthScopes cannot contain empty values.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
