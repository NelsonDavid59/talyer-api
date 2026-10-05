using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence.Repositories;

public class TenantRep : ITenantRep
{
    private readonly ApplicationDbContext _context;

    public TenantRep(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Tenant>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FindAsync(id, cancellationToken);

        if (tenant is null)
        {
            return Result<Tenant>.Failure(DomainErrors.Tenant.TenantNotFound);
        }

        return Result<Tenant>.Success(tenant);
    }
}
