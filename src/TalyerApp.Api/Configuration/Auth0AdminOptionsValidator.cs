using Microsoft.Extensions.Options;
using TalyerApp.Infrastructure.Identity;

namespace TalyerApp.Api.Configuration;

public sealed class Auth0AdminOptionsValidator : IValidateOptions<Auth0AdminOptions>
{
    private readonly Auth0SharedOptions _shared;

    public Auth0AdminOptionsValidator(IOptions<Auth0SharedOptions> shared)
    {
        _shared = shared.Value;
    }

    public ValidateOptionsResult Validate(string? name, Auth0AdminOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ClientId))
        {
            failures.Add($"{Auth0AdminOptions.SectionName}:ClientId is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ClientSecret))
        {
            failures.Add(
                $"{Auth0AdminOptions.SectionName}:ClientSecret is required. " +
                "Set a non-empty value in usersecrets.json (Development) or Auth0__Admin__ClientSecret.");
        }

        if (string.IsNullOrWhiteSpace(Auth0ConnectionResolver.ResolveBaseUrl(options.BaseUrl, _shared.BaseUrl)))
        {
            failures.Add(
                $"{Auth0AdminOptions.SectionName}:BaseUrl is required (set Auth0:Admin:BaseUrl or Auth0:BaseUrl).");
        }

        if (string.IsNullOrWhiteSpace(Auth0ConnectionResolver.ResolveRealm(options.Realm, _shared.Realm)))
        {
            failures.Add(
                $"{Auth0AdminOptions.SectionName}:Realm is required (set Auth0:Admin:Realm or Auth0:Realm).");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
