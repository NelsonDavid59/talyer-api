using Microsoft.AspNetCore.Authentication.JwtBearer;
using TalyerApp.Api.Configuration;

namespace TalyerApp.Api.DependencyInjection;

public static class ConfigureJwtBearerOptions
{
    public static void Apply(
        JwtBearerOptions options,
        KeycloakApiOptions api,
        KeycloakSharedOptions shared)
    {
        options.Authority = KeycloakUrlBuilder.GetAuthority(api, shared);
        options.Audience = api.Audience!;
        options.RequireHttpsMetadata = api.RequireHttpsMetadata!.Value;
    }
}
