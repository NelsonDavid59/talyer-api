using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Authorization;
using TalyerApp.Domain.Shared.Result;
using TalyerApp.Domain.Shared.Tenancy;

namespace TalyerApp.Application.Common.Authorization;

public class PlatformAdminAuthorizer : IPlatformAdminAuthorizer
{
    private readonly IRoleRep _roleRepository;
    private readonly ITenantRep _tenantRepository;
    private readonly IUserRoleAssignmentRep _userRoleAssignmentRepository;

    public PlatformAdminAuthorizer(
        IRoleRep roleRepository,
        ITenantRep tenantRepository,
        IUserRoleAssignmentRep userRoleAssignmentRepository)
    {
        _roleRepository = roleRepository;
        _tenantRepository = tenantRepository;
        _userRoleAssignmentRepository = userRoleAssignmentRepository;
    }

    public async Task<Result> EnsureIsPlatformAdminAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByCodeAsync(RoleCodes.PlatformAdmin, cancellationToken);
        if (roleResult.IsFailure)
        {
            return Result.Failure(roleResult.Error);
        }

        var tenantResult = await _tenantRepository.GetByCodeAsync(TenantCodeConstants.Platform, cancellationToken);
        if (tenantResult.IsFailure)
        {
            return Result.Failure(tenantResult.Error);
        }

        var exists = await _userRoleAssignmentRepository.ExistsAsync(
            userId,
            roleResult.Value.Id,
            tenantResult.Value.Id,
            branchId: null,
            cancellationToken);

        return exists
            ? Result.Success()
            : Result.Failure(DomainErrors.Authorization.Forbidden);
    }
}
