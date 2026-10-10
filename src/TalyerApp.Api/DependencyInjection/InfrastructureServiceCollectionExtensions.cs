using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TalyerApp.Api.Configuration;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Infrastructure.Identity;
using TalyerApp.Infrastructure.Notifications;
using TalyerApp.Infrastructure.Persistence;
using TalyerApp.Infrastructure.Persistence.Repositories;
using TalyerApp.Infrastructure.Persistence.Seeding;

namespace TalyerApp.Api.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRep, UserRep>();
        services.AddScoped<IExternalIdentityRep, ExternalIdentityRep>();
        services.AddScoped<IUserRoleAssignmentRep, UserRoleAssignmentRep>();
        services.AddScoped<IRoleRep, RoleRep>();
        services.AddScoped<ITenantRep, TenantRep>();
        services.AddScoped<IBranchRep, BranchRep>();
        services.AddScoped<IMembershipRequestRep, MembershipRequestRep>();

        services.AddScoped<IEmailSender, LoggingEmailSender>();
        services.AddHttpClient<IIdentityAdminService, KeycloakIdentityAdminService>((sp, client) =>
        {
            var adminOptions = sp.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;
            var sharedOptions = sp.GetRequiredService<IOptions<KeycloakSharedOptions>>().Value;
            var baseUrl = KeycloakConnectionResolver.GetNormalizedBaseUrl(
                adminOptions.BaseUrl,
                sharedOptions.BaseUrl);
            client.BaseAddress = new Uri(baseUrl + "/");
        });

        services.AddSingleton<IUniqueConstraintRegistry>(_ => UniqueConstraintRegistry.Create());
        services.AddScoped<IDatabaseExceptionTranslator, PostgresDatabaseExceptionTranslator>();

        services.AddScoped<RbacReferenceDataSeeder>();
        services.AddScoped<PlatformTenantReferenceDataSeeder>();
        services.AddScoped<IReferenceDataSeeder, ReferenceDataSeeder>();
        services.AddScoped<IPlatformAdminBootstrapSeeder, PlatformAdminBootstrapSeeder>();

        return services;
    }
}
