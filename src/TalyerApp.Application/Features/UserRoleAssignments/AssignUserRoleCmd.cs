using TalyerApp.Application.Common.Interfaces.CQRS;

namespace TalyerApp.Application.Features.UserRoleAssignments;

public sealed record AssignUserRoleCmd(
    Guid UserId,
    int RoleId,
    int TenantId,
    int? BranchId
) : ICommand<int>;
