using Microsoft.Extensions.Options;

namespace TalyerApp.Api.Configuration;

public sealed class KeycloakSharedOptionsValidator : IValidateOptions<KeycloakSharedOptions>
{
    public ValidateOptionsResult Validate(string? name, KeycloakSharedOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            failures.Add($"{KeycloakSharedOptions.SectionName}:BaseUrl is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Realm))
        {
            failures.Add($"{KeycloakSharedOptions.SectionName}:Realm is required.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
