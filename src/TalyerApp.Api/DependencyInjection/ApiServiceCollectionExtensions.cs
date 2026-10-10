using Microsoft.Extensions.Options;
using TalyerApp.Api.Configuration;
using TalyerApp.Api.Localization;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Infrastructure.Identity;
using TalyerApp.Infrastructure.Persistence.Seeding;

namespace TalyerApp.Api.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PlatformAdminBootstrapOptions>(
            configuration.GetSection(PlatformAdminBootstrapOptions.SectionName));

        services.AddSingleton<IValidateOptions<KeycloakSharedOptions>, KeycloakSharedOptionsValidator>();
        services.AddOptions<KeycloakSharedOptions>()
            .Bind(configuration.GetSection(KeycloakSharedOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<KeycloakApiOptions>, KeycloakApiOptionsValidator>();
        services.AddOptions<KeycloakApiOptions>()
            .Bind(configuration.GetSection(KeycloakApiOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<KeycloakDocsOptions>, KeycloakDocsOptionsValidator>();
        services.AddOptions<KeycloakDocsOptions>()
            .Bind(configuration.GetSection(KeycloakDocsOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<KeycloakAdminOptions>, KeycloakAdminOptionsValidator>();
        services.AddOptions<KeycloakAdminOptions>()
            .Bind(configuration.GetSection(KeycloakAdminOptions.SectionName))
            .PostConfigure<IOptions<KeycloakSharedOptions>>((admin, sharedOptions) =>
            {
                var shared = sharedOptions.Value;
                if (string.IsNullOrWhiteSpace(admin.BaseUrl))
                {
                    admin.BaseUrl = shared.BaseUrl;
                }

                if (string.IsNullOrWhiteSpace(admin.Realm))
                {
                    admin.Realm = shared.Realm;
                }
            })
            .ValidateOnStart();

        services.AddSingleton<ErrorCatalog>();
        services.AddSingleton<IErrorMessageResolver, JsonErrorMessageResolver>();
        services.AddSingleton<IErrorHttpStatusMapper, ErrorHttpStatusMapper>();
        services.AddSingleton<ErrorCatalogValidator>();

        return services;
    }
}
