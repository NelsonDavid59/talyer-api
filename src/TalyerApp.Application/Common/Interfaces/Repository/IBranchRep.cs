using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface IBranchRep
{
    Task<Result<Branch>> GetByTenantAndIdAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default);
}
