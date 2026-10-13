using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using TalyerApp.Api.Configuration;

namespace TalyerApp.Api.DependencyInjection;

public static class OpenApiServiceCollectionExtensions
{
    public static IServiceCollection AddTalyerOpenApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                var apiOptions = context.ApplicationServices
                    .GetRequiredService<IOptions<KeycloakApiOptions>>()
                    .Value;
                var sharedOptions = context.ApplicationServices
                    .GetRequiredService<IOptions<KeycloakSharedOptions>>()
                    .Value;
                var docsOptions = context.ApplicationServices
                    .GetRequiredService<IOptions<KeycloakDocsOptions>>()
                    .Value;

                var authorizationUrl = KeycloakUrlBuilder.GetAuthorizationEndpoint(apiOptions, sharedOptions);
                var tokenUrl = KeycloakUrlBuilder.GetTokenEndpoint(apiOptions, sharedOptions);

                var scopes = docsOptions.OAuthScopes!
                    .ToDictionary(
                        scope => scope,
                        scope => scope switch
                        {
                            "openid" => "OpenID",
                            "profile" => "User profile",
                            "email" => "User email",
                            _ => scope
                        });

                document.Components ??= new();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes["OAuth2"] =
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Description = "Keycloak OAuth2 Authorization Code + PKCE",
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(authorizationUrl),
                                TokenUrl = new Uri(tokenUrl),
                                Scopes = scopes
                            }
                        }
                    };

                return Task.CompletedTask;
            });
        });

        return services;
    }
}
