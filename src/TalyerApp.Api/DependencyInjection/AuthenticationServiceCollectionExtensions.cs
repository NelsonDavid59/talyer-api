using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TalyerApp.Api.Configuration;
using TalyerApp.Api.Identity;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Domain.Authorization;
using AuthSchemes = TalyerApp.Domain.Authorization.AuthenticationSchemes;

namespace TalyerApp.Api.DependencyInjection;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddTalyerAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddSingleton<ITalyerSessionTokenIssuer, TalyerSessionTokenIssuer>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthSchemes.Talyer;
                options.DefaultChallengeScheme = AuthSchemes.Talyer;
            })
            .AddJwtBearer(AuthSchemes.Talyer, options => { })
            .AddJwtBearer(AuthSchemes.Auth0, options => { });

        services.AddOptions<JwtBearerOptions>(AuthSchemes.Talyer)
            .Configure<IOptions<TalyerSessionOptions>, IHostEnvironment>((jwt, sessionOptions, environment) =>
            {
                var session = sessionOptions.Value;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = session.Issuer,
                    ValidateAudience = true,
                    ValidAudience = session.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(session.SigningKey!)),
                    ValidateLifetime = true,
                    NameClaimType = System.Security.Claims.ClaimTypes.Name,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
                jwt.RequireHttpsMetadata = !environment.IsDevelopment();
            });

        services.AddOptions<JwtBearerOptions>(AuthSchemes.Auth0)
            .Configure<IOptions<Auth0ApiOptions>, IOptions<Auth0SharedOptions>>((jwt, apiOptions, sharedOptions) =>
                ConfigureJwtBearerOptions.Apply(jwt, apiOptions.Value, sharedOptions.Value));

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.PlatformOwner,
                policy => policy.RequireRole(RoleCodes.PlatformOwner));
        });

        return services;
    }
}
