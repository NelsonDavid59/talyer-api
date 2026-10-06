using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Seeding;

public class PlatformTenantReferenceDataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PlatformTenantReferenceDataSeeder> _logger;

    public PlatformTenantReferenceDataSeeder(
        ApplicationDbContext context,
        ILogger<PlatformTenantReferenceDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var exists = await _context.Tenants
            .AnyAsync(t => t.Code == PlatformTenantCatalog.Code, cancellationToken);

        if (exists)
        {
            _logger.LogDebug(
                "Platform tenant '{Code}' already exists.",
                PlatformTenantCatalog.Code);
            return;
        }

        var result = Tenant.CreatePlatform(
            PlatformTenantCatalog.Description,
            PlatformTenantCatalog.Code);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Invalid platform tenant in catalog ({result.Error.Code}).");
        }

        _context.Tenants.Add(result.Value);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Inserted platform tenant '{Code}'.",
            PlatformTenantCatalog.Code);
    }
}
