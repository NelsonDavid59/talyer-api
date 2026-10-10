using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Application.Features.ExternalIdentities;
using TalyerApp.Application.Interfaces;
using TalyerApp.Api.Configuration;

namespace TalyerApp.Api.DependencyInjection;

public static class AuthenticationServiceCollectionExtensions
{
    /// <summary>
    /// Adds authentication services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTalyerAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var principal = context.Principal;
                        var providerUserId = principal?.FindFirst("sub")?.Value;
                        var email = principal?.FindFirst("email")?.Value;
                        var username = principal?.FindFirst("preferred_username")?.Value;
                        var firstName = principal?.FindFirst("given_name")?.Value;
                        var lastName = principal?.FindFirst("family_name")?.Value;

                        if (string.IsNullOrWhiteSpace(providerUserId) ||
                            string.IsNullOrWhiteSpace(email) ||
                            string.IsNullOrWhiteSpace(username) ||
                            string.IsNullOrWhiteSpace(firstName) ||
                            string.IsNullOrWhiteSpace(lastName))
                        {
                            context.Fail("The token does not contain the required user claims.");
                            return;
                        }

                        var dispatcher = context.HttpContext.RequestServices
                            .GetRequiredService<ICommandDispatcher>();

                        var command = new RegisterExternalIdentityCmd(
                            Provider: "keycloak",
                            ProviderUserId: providerUserId,
                            Username: username,
                            FirstName: firstName,
                            LastName: lastName,
                            Email: email);

                        var result = await dispatcher.DispatchAsync<RegisterExternalIdentityCmd, Guid>(
                            command,
                            context.HttpContext.RequestAborted);

                        if (result.IsFailure)
                        {
                            var messageResolver = context.HttpContext.RequestServices
                                .GetRequiredService<IErrorMessageResolver>();
                            context.Fail(messageResolver.ResolveMessage(result.Error));
                            return;
                        }

                        principal!.AddIdentity(new ClaimsIdentity(
                        [
                            new Claim("talyer_user_id", result.Value.ToString())
                        ]));
                    }
                };
            });

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<KeycloakApiOptions>, IOptions<KeycloakSharedOptions>>((jwt, apiOptions, sharedOptions) =>
                ConfigureJwtBearerOptions.Apply(jwt, apiOptions.Value, sharedOptions.Value));

        services.AddAuthorization();

        return services;
    }
}
