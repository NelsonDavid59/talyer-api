using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface ITenantRep
{
    Task<Result<Tenant>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<Tenant>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByDescriptionAsync(string description, CancellationToken cancellationToken = default);

    void Add(Tenant tenant);
}
