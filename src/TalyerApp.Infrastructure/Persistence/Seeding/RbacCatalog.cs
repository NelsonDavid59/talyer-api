using TalyerApp.Domain.Authorization;

namespace TalyerApp.Infrastructure.Persistence.Seeding;

public static class RbacCatalog
{
    public sealed record PermissionDefinition(string Code);

    public sealed record RoleDefinition(string Code, string RoleName);

    public static IReadOnlyList<PermissionDefinition> Permissions =>
    [
        new(PermissionCodes.TenantsManage),
        new(PermissionCodes.BranchesManage),
        new(PermissionCodes.UsersAssignRoles),
        new(PermissionCodes.UsersInvite),
    ];

    public static IReadOnlyList<RoleDefinition> Roles =>
    [
        new(RoleCodes.PlatformAdmin, "Platform Administrator"),
        new(RoleCodes.TenantAdmin, "Tenant Administrator"),
    ];

    /// <summary>
    /// Role code to permission codes granted to that role.
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> RolePermissions =>
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [RoleCodes.PlatformAdmin] =
            [
                PermissionCodes.TenantsManage,
                PermissionCodes.BranchesManage,
                PermissionCodes.UsersAssignRoles,
                PermissionCodes.UsersInvite,
            ],
            [RoleCodes.TenantAdmin] =
            [
                PermissionCodes.BranchesManage,
                PermissionCodes.UsersAssignRoles,
                PermissionCodes.UsersInvite,
            ],
        };
}
