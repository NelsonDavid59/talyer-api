using Microsoft.Extensions.Options;

namespace TalyerApp.Api.Configuration;

public sealed class Auth0ApiOptionsValidator : IValidateOptions<Auth0ApiOptions>
{
    private readonly Auth0SharedOptions _shared;

    public Auth0ApiOptionsValidator(IOptions<Auth0SharedOptions> shared)
    {
        _shared = shared.Value;
    }

    public ValidateOptionsResult Validate(string? name, Auth0ApiOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add($"{Auth0ApiOptions.SectionName}:Audience is required.");
        }

        if (options.RequireHttpsMetadata is null)
        {
            failures.Add($"{Auth0ApiOptions.SectionName}:RequireHttpsMetadata must be set explicitly (true or false).");
        }

        var hasAuthority = !string.IsNullOrWhiteSpace(options.Authority);
        var effectiveBaseUrl = Auth0ConnectionResolver.ResolveBaseUrl(options.BaseUrl, _shared.BaseUrl);
        var effectiveRealm = Auth0ConnectionResolver.ResolveRealm(options.Realm, _shared.Realm);
        var hasBaseUrlAndRealm =
            !string.IsNullOrWhiteSpace(effectiveBaseUrl) &&
            !string.IsNullOrWhiteSpace(effectiveRealm);

        if (!hasAuthority && !hasBaseUrlAndRealm)
        {
            failures.Add(
                $"{Auth0ApiOptions.SectionName}: set Authority, or configure Auth0:BaseUrl and Auth0:Realm, or Auth0:Api:BaseUrl and Auth0:Api:Realm.");
        }

        if (!hasAuthority)
        {
            if (!string.IsNullOrWhiteSpace(options.BaseUrl) && string.IsNullOrWhiteSpace(options.Realm) &&
                string.IsNullOrWhiteSpace(_shared.Realm))
            {
                failures.Add($"{Auth0ApiOptions.SectionName}:Realm is required when Api BaseUrl is set without Auth0:Realm.");
            }

            if (string.IsNullOrWhiteSpace(options.BaseUrl) && !string.IsNullOrWhiteSpace(options.Realm) &&
                string.IsNullOrWhiteSpace(_shared.BaseUrl))
            {
                failures.Add($"{Auth0ApiOptions.SectionName}:BaseUrl is required when Api Realm is set without Auth0:BaseUrl.");
            }
        }

        try
        {
            if (failures.Count == 0)
            {
                Auth0UrlBuilder.GetAuthority(options, _shared);
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
