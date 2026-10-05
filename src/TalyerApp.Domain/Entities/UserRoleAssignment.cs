using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class UserRoleAssignment : BaseEntity
{
    public int Id { get; private set; }
    public Guid UserId { get; private set; }
    public int RoleId { get; private set; }
    public int TenantId { get; private set; }
    public int? BranchId { get; private set; }

    public User User { get; set; }
    public Role Role { get; set; }
    public Tenant Tenant { get; set; }
    public Branch? Branch { get; set; }

    private UserRoleAssignment(Guid userId, int roleId, int tenantId, int? branchId)
    {
        UserId = userId;
        RoleId = roleId;
        TenantId = tenantId;
        BranchId = branchId;
    }

    public static Result<UserRoleAssignment> Create(Guid userId, int roleId, int tenantId, int? branchId)
    {
        if (userId == Guid.Empty)
        {
            return Result<UserRoleAssignment>.Failure(DomainErrors.UserRoleAssignment.InvalidUserId);
        }

        if (roleId <= 0)
        {
            return Result<UserRoleAssignment>.Failure(DomainErrors.UserRoleAssignment.InvalidRoleId);
        }

        if (tenantId <= 0)
        {
            return Result<UserRoleAssignment>.Failure(DomainErrors.UserRoleAssignment.InvalidTenantId);
        }

        if (branchId.HasValue && branchId.Value <= 0)
        {
            return Result<UserRoleAssignment>.Failure(DomainErrors.UserRoleAssignment.InvalidBranchId);
        }

        return Result<UserRoleAssignment>.Success(new UserRoleAssignment(
            userId,
            roleId,
            tenantId,
            branchId));
    }
}