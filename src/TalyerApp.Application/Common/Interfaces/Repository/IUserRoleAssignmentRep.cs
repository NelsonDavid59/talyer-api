using TalyerApp.Domain.Entities;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface IUserRoleAssignmentRep
{
    Task<bool> ExistsAsync(
        Guid userId,
        int roleId,
        int tenantId,
        int? branchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserRoleAssignment>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    void Add(UserRoleAssignment assignment);
}
