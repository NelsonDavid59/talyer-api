using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence;

public static class UniqueConstraintMappings
{
    public static IReadOnlyList<UniqueConstraintRegistration> GetConstraintRegistrations() =>
    [
        new(UniqueConstraintNames.UserRoleAssignmentTenantWide, DomainErrors.UserRoleAssignment.AlreadyExists),
        new(UniqueConstraintNames.UserRoleAssignmentPerBranch, DomainErrors.UserRoleAssignment.AlreadyExists),
        new(UniqueConstraintNames.ExternalIdentityProviderProviderUserId, DomainErrors.ExternalIdentity.AlreadyExists),
        new(UniqueConstraintNames.RolePermissionRoleIdPermissionId, DomainErrors.RolePermission.AlreadyExists),
        new(UniqueConstraintNames.BranchTenantIdId, DomainErrors.Branch.AlreadyExists),
        new(UniqueConstraintNames.UserEmail, DomainErrors.User.AlreadyExists)
    ];

    public static IReadOnlyList<UniqueConstraintTableFallback> GetTableFallbacks() =>
    [
        new("UserRoleAssignments", DomainErrors.UserRoleAssignment.AlreadyExists),
        new("ExternalIdentities", DomainErrors.ExternalIdentity.AlreadyExists),
        new("RolePermissions", DomainErrors.RolePermission.AlreadyExists),
        new("Branches", DomainErrors.Branch.AlreadyExists),
        new("Users", DomainErrors.User.AlreadyExists)
    ];
}
