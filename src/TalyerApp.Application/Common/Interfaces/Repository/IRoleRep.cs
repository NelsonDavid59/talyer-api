using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface IRoleRep
{
    Task<Result<Role>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
