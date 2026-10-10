using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using TalyerApp.Api.Configuration;

namespace TalyerApp.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseTalyerPipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    public static WebApplication MapTalyerDevelopmentApiDocs(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        var docsOptions = app.Services.GetRequiredService<IOptions<KeycloakDocsOptions>>().Value;

        app.MapOpenApi();
        app.MapScalarApiReference(docsOptions.ScalarRoute!, options =>
        {
            options
                .AddPreferredSecuritySchemes("OAuth2")
                .AddAuthorizationCodeFlow("OAuth2", flow =>
                {
                    flow.ClientId = docsOptions.OAuthClientId!;
                    flow.Pkce = Pkce.Sha256;

                    flow.AddBodyParameter(
                        "client_id",
                        docsOptions.OAuthClientId!);

                    flow.SelectedScopes = docsOptions.OAuthScopes!;
                });
        });

        return app;
    }
}
