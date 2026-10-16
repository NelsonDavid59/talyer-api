using Microsoft.Extensions.Options;

namespace TalyerApp.Api.Configuration;

public sealed class Auth0SharedOptionsValidator : IValidateOptions<Auth0SharedOptions>
{
    public ValidateOptionsResult Validate(string? name, Auth0SharedOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            failures.Add($"{Auth0SharedOptions.SectionName}:BaseUrl is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Realm))
        {
            failures.Add($"{Auth0SharedOptions.SectionName}:Realm is required.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
