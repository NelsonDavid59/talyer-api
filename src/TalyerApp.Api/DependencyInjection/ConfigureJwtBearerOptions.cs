using Microsoft.AspNetCore.Authentication.JwtBearer;
using TalyerApp.Api.Configuration;

namespace TalyerApp.Api.DependencyInjection;

public static class ConfigureJwtBearerOptions
{
    public static void Apply(
        JwtBearerOptions options,
        Auth0ApiOptions api,
        Auth0SharedOptions shared)
    {
        options.Authority = Auth0UrlBuilder.GetAuthority(api, shared);
        options.Audience = api.Audience!;
        options.RequireHttpsMetadata = api.RequireHttpsMetadata!.Value;
    }
}
