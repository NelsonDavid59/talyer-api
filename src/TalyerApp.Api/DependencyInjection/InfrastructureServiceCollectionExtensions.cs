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

        services.AddSingleton<IEmailTemplateRenderer, FileEmailTemplateRenderer>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddHttpClient<IIdentityAdminService, Auth0IdentityAdminService>((sp, client) =>
        {
            var adminOptions = sp.GetRequiredService<IOptions<Auth0AdminOptions>>().Value;
            var sharedOptions = sp.GetRequiredService<IOptions<Auth0SharedOptions>>().Value;
            var baseUrl = Auth0ConnectionResolver.GetNormalizedBaseUrl(
                adminOptions.BaseUrl,
                sharedOptions.BaseUrl);
            client.BaseAddress = new Uri(baseUrl + "/");
        });

        services.AddSingleton<IUniqueConstraintRegistry>(_ => UniqueConstraintRegistry.Create());
        services.AddScoped<IDatabaseExceptionTranslator, PostgresDatabaseExceptionTranslator>();

        services.AddScoped<RbacReferenceDataSeeder>();
        services.AddScoped<PlatformTenantReferenceDataSeeder>();
        services.AddScoped<IReferenceDataSeeder, ReferenceDataSeeder>();
        services.AddScoped<IPlatformOwnerBootstrapSeeder, PlatformOwnerBootstrapSeeder>();

        return services;
    }
}
