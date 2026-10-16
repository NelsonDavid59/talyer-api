using Microsoft.Extensions.Options;
using TalyerApp.Api.Configuration;
using TalyerApp.Api.Localization;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Infrastructure.Identity;
using TalyerApp.Infrastructure.Notifications;
using TalyerApp.Infrastructure.Persistence.Seeding;

namespace TalyerApp.Api.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PlatformOwnerBootstrapOptions>(
            configuration.GetSection(PlatformOwnerBootstrapOptions.SectionName));

        services.AddSingleton<IValidateOptions<Auth0SharedOptions>, Auth0SharedOptionsValidator>();
        services.AddOptions<Auth0SharedOptions>()
            .Bind(configuration.GetSection(Auth0SharedOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<Auth0ApiOptions>, Auth0ApiOptionsValidator>();
        services.AddOptions<Auth0ApiOptions>()
            .Bind(configuration.GetSection(Auth0ApiOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<Auth0DocsOptions>, Auth0DocsOptionsValidator>();
        services.AddOptions<Auth0DocsOptions>()
            .Bind(configuration.GetSection(Auth0DocsOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<Auth0AdminOptions>, Auth0AdminOptionsValidator>();
        services.AddOptions<Auth0AdminOptions>()
            .Bind(configuration.GetSection(Auth0AdminOptions.SectionName))
            .PostConfigure<IOptions<Auth0SharedOptions>>((admin, sharedOptions) =>
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

        services.AddSingleton<IValidateOptions<TalyerSessionOptions>, TalyerSessionOptionsValidator>();
        services.AddOptions<TalyerSessionOptions>()
            .Bind(configuration.GetSection(TalyerSessionOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<SmtpOptions>, SmtpOptionsValidator>();
        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<ErrorCatalog>();
        services.AddSingleton<IErrorMessageResolver, JsonErrorMessageResolver>();
        services.AddSingleton<IErrorHttpStatusMapper, ErrorHttpStatusMapper>();
        services.AddSingleton<ErrorCatalogValidator>();
        services.AddSingleton<EmailTemplateValidator>();

        return services;
    }
}
