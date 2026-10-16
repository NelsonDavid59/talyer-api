using Microsoft.Extensions.Options;

namespace TalyerApp.Api.Configuration;

public sealed class Auth0DocsOptionsValidator : IValidateOptions<Auth0DocsOptions>
{
    public ValidateOptionsResult Validate(string? name, Auth0DocsOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ScalarRoute))
        {
            failures.Add($"{Auth0DocsOptions.SectionName}:ScalarRoute is required.");
        }
        else if (!options.ScalarRoute.StartsWith("/", StringComparison.Ordinal))
        {
            failures.Add($"{Auth0DocsOptions.SectionName}:ScalarRoute must start with '/'.");
        }

        if (string.IsNullOrWhiteSpace(options.OAuthClientId))
        {
            failures.Add($"{Auth0DocsOptions.SectionName}:OAuthClientId is required.");
        }

        if (options.OAuthScopes is null || options.OAuthScopes.Length == 0)
        {
            failures.Add($"{Auth0DocsOptions.SectionName}:OAuthScopes must contain at least one scope.");
        }
        else if (options.OAuthScopes.Any(string.IsNullOrWhiteSpace))
        {
            failures.Add($"{Auth0DocsOptions.SectionName}:OAuthScopes cannot contain empty values.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
