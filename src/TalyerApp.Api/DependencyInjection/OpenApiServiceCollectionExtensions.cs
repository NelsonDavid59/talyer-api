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
                    .GetRequiredService<IOptions<Auth0ApiOptions>>()
                    .Value;
                var sharedOptions = context.ApplicationServices
                    .GetRequiredService<IOptions<Auth0SharedOptions>>()
                    .Value;
                var docsOptions = context.ApplicationServices
                    .GetRequiredService<IOptions<Auth0DocsOptions>>()
                    .Value;

                var authorizationUrl = Auth0UrlBuilder.GetAuthorizationEndpoint(apiOptions, sharedOptions);
                var tokenUrl = Auth0UrlBuilder.GetTokenEndpoint(apiOptions, sharedOptions);

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
                        Description = "Auth0 OAuth2 for POST /session (Magic Link / Authorization Code + PKCE)",
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

                document.Components.SecuritySchemes["TalyerJwt"] =
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Talyer session JWT from POST /session"
                    };

                return Task.CompletedTask;
            });
        });

        return services;
    }
}
