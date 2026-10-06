namespace TalyerApp.Infrastructure.Persistence.Seeding;

public static class RbacCatalog
{
    public sealed record PermissionDefinition(string Code);

    public sealed record RoleDefinition(string Code, string RoleName);

    public static IReadOnlyList<PermissionDefinition> Permissions =>
    [
        new("tenants.manage"),
        new("branches.manage"),
        new("users.assign_roles"),
    ];

    public static IReadOnlyList<RoleDefinition> Roles =>
    [
        new("platform_admin", "Platform Administrator"),
        new("tenant_admin", "Tenant Administrator"),
        new("member", "Member"),
    ];

    /// <summary>
    /// Role code to permission codes granted to that role.
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> RolePermissions =>
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["platform_admin"] =
            [
                "tenants.manage",
                "branches.manage",
                "users.assign_roles",
            ],
            ["tenant_admin"] =
            [
                "branches.manage",
                "users.assign_roles",
            ],
            ["member"] = [],
        };
}
