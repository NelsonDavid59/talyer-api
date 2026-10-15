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
        new(RoleCodes.PlatformOwner, "Platform Owner"),
        new(RoleCodes.TenantOwner, "Tenant Owner"),
    ];

    /// <summary>
    /// Role code to permission codes granted to that role.
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> RolePermissions =>
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [RoleCodes.PlatformOwner] =
            [
                PermissionCodes.TenantsManage,
                PermissionCodes.BranchesManage,
                PermissionCodes.UsersAssignRoles,
                PermissionCodes.UsersInvite,
            ],
            [RoleCodes.TenantOwner] =
            [
                PermissionCodes.BranchesManage,
                PermissionCodes.UsersAssignRoles,
                PermissionCodes.UsersInvite,
            ],
        };
}
