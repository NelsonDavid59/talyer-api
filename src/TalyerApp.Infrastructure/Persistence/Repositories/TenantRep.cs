using Microsoft.EntityFrameworkCore;
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

    public async Task<Result<Tenant>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);

        if (tenant is null)
        {
            return Result<Tenant>.Failure(DomainErrors.Tenant.TenantNotFound);
        }

        return Result<Tenant>.Success(tenant);
    }

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        _context.Tenants.AnyAsync(t => t.Code == code, cancellationToken);

    public Task<bool> ExistsByDescriptionAsync(string description, CancellationToken cancellationToken = default) =>
        _context.Tenants.AnyAsync(t => t.Description == description, cancellationToken);

    public void Add(Tenant tenant)
    {
        _context.Tenants.Add(tenant);
    }
}
