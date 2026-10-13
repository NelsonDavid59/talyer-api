using TalyerApp.Application.Common.Interfaces.Persistence;

namespace TalyerApp.Infrastructure.Persistence.Seeding;

public class ReferenceDataSeeder : IReferenceDataSeeder
{
    private readonly RbacReferenceDataSeeder _rbacReferenceDataSeeder;
    private readonly PlatformTenantReferenceDataSeeder _platformTenantReferenceDataSeeder;

    public ReferenceDataSeeder(
        RbacReferenceDataSeeder rbacReferenceDataSeeder,
        PlatformTenantReferenceDataSeeder platformTenantReferenceDataSeeder)
    {
        _rbacReferenceDataSeeder = rbacReferenceDataSeeder;
        _platformTenantReferenceDataSeeder = platformTenantReferenceDataSeeder;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _rbacReferenceDataSeeder.SeedAsync(cancellationToken);
        await _platformTenantReferenceDataSeeder.SeedAsync(cancellationToken);
    }
}
