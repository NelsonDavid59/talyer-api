using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface ITenantRep
{
    Task<Result<Tenant>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
