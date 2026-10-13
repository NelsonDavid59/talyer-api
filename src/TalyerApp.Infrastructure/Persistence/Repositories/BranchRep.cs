using Microsoft.EntityFrameworkCore;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence.Repositories;

public class BranchRep : IBranchRep
{
    private readonly ApplicationDbContext _context;

    public BranchRep(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Branch>> GetByTenantAndIdAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        var branch = await _context.Branches
            .FirstOrDefaultAsync(
                b => b.TenantId == tenantId && b.Id == branchId,
                cancellationToken);

        if (branch is null)
        {
            return Result<Branch>.Failure(DomainErrors.Branch.BranchNotFound);
        }

        return Result<Branch>.Success(branch);
    }
}
