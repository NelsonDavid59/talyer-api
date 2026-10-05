namespace TalyerApp.Infrastructure.Persistence;

public static class UniqueConstraintNames
{
    public const string UserRoleAssignmentTenantWide =
        "IX_UserRoleAssignments_UserId_RoleId_TenantId_TenantWide";

    public const string UserRoleAssignmentPerBranch =
        "IX_UserRoleAssignments_UserId_RoleId_TenantId_BranchId_PerBranch";

    public const string ExternalIdentityProviderProviderUserId =
        "IX_ExternalIdentities_Provider_ProviderUserId";

    public const string RolePermissionRoleIdPermissionId =
        "IX_RolePermissions_RoleId_PermissionId";

    public const string BranchTenantIdId =
        "IX_Branches_TenantId_Id";
}
