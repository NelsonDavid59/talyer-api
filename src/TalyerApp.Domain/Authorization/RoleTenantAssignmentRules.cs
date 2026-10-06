using TalyerApp.Domain.Shared.Tenancy;

namespace TalyerApp.Domain.Authorization;

public static class RoleTenantAssignmentRules
{
    public static bool IsAllowed(string roleCode, TenantType tenantType, int? branchId)
    {
        if (string.Equals(roleCode, RoleCodes.PlatformAdmin, StringComparison.Ordinal))
        {
            return tenantType == TenantType.Platform && branchId is null;
        }

        if (string.Equals(roleCode, RoleCodes.TenantAdmin, StringComparison.Ordinal) ||
            string.Equals(roleCode, RoleCodes.Member, StringComparison.Ordinal))
        {
            return tenantType == TenantType.Customer;
        }

        return false;
    }
}
