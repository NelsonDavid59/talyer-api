using Microsoft.Extensions.Logging;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Authorization;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Domain.Shared.Result;
using TalyerApp.Domain.Shared.Tenancy;

namespace TalyerApp.Application.Features.MembershipRequests;

public class ApproveMembershipRequestCmdHdlr : ICommandHandler<ApproveMembershipRequestCmd, int>
{
    private readonly IMembershipRequestRep _membershipRequestRepository;
    private readonly ITenantRep _tenantRepository;
    private readonly IUserRep _userRepository;
    private readonly IExternalIdentityRep _externalIdentityRepository;
    private readonly IRoleRep _roleRepository;
    private readonly IUserRoleAssignmentRep _userRoleAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityAdminService _identityAdminService;
    private readonly ILogger<ApproveMembershipRequestCmdHdlr> _logger;

    public ApproveMembershipRequestCmdHdlr(
        IMembershipRequestRep membershipRequestRepository,
        ITenantRep tenantRepository,
        IUserRep userRepository,
        IExternalIdentityRep externalIdentityRepository,
        IRoleRep roleRepository,
        IUserRoleAssignmentRep userRoleAssignmentRepository,
        IUnitOfWork unitOfWork,
        IIdentityAdminService identityAdminService,
        ILogger<ApproveMembershipRequestCmdHdlr> logger)
    {
        _membershipRequestRepository = membershipRequestRepository;
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _externalIdentityRepository = externalIdentityRepository;
        _roleRepository = roleRepository;
        _userRoleAssignmentRepository = userRoleAssignmentRepository;
        _unitOfWork = unitOfWork;
        _identityAdminService = identityAdminService;
        _logger = logger;
    }

    public async Task<Result<int>> HandleAsync(
        ApproveMembershipRequestCmd command,
        CancellationToken cancellationToken = default)
    {
        var requestResult = await _membershipRequestRepository.GetByIdAsync(command.Id, cancellationToken);
        if (requestResult.IsFailure)
        {
            return Result<int>.Failure(requestResult.Error);
        }

        var request = requestResult.Value;
        if (request.Status != MembershipRequestStatus.PendingReview)
        {
            return Result<int>.Failure(DomainErrors.MembershipRequest.InvalidStatus);
        }

        if (await _tenantRepository.ExistsByDescriptionAsync(request.CompanyName, cancellationToken))
        {
            request.Reject("Tenant description already exists");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Failure(DomainErrors.Tenant.AlreadyExists);
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken) ||
            await _identityAdminService.UserExistsByEmailAsync(request.Email, cancellationToken))
        {
            request.Reject("email already registered");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Failure(DomainErrors.MembershipRequest.AlreadyExists);
        }

        var codeResult = await AllocateTenantCodeAsync(request.CompanyName, cancellationToken);
        if (codeResult.IsFailure)
        {
            return Result<int>.Failure(codeResult.Error);
        }

        var tenantResult = Tenant.CreateCustomer(request.CompanyName, codeResult.Value);
        if (tenantResult.IsFailure)
        {
            return Result<int>.Failure(tenantResult.Error);
        }

        var roleResult = await _roleRepository.GetByCodeAsync(RoleCodes.TenantAdmin, cancellationToken);
        if (roleResult.IsFailure)
        {
            return Result<int>.Failure(roleResult.Error);
        }

        var createUserResult = await _identityAdminService.CreateUserAsync(
            request.Email,
            request.Email,
            request.FirstName,
            request.LastName,
            cancellationToken);

        if (createUserResult.IsFailure)
        {
            if (createUserResult.Error.Code == ErrorCodes.IdentityAdmin.UserAlreadyExists)
            {
                request.Reject("email already registered");
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Failure(DomainErrors.IdentityAdmin.UserAlreadyExists);
            }

            return Result<int>.Failure(createUserResult.Error);
        }

        var providerUserId = createUserResult.Value;

        var emailResult = await _identityAdminService.SendExecuteActionsEmailAsync(
            providerUserId,
            ["UPDATE_PASSWORD"],
            cancellationToken);

        if (emailResult.IsFailure)
        {
            await CompensateKeycloakUserAsync(providerUserId, cancellationToken);
            _logger.LogError(
                "Membership request {MembershipRequestId} approval rolled back: Keycloak invitation email failed for {ProviderUserId}. Request remains PendingReview.",
                request.Id,
                providerUserId);
            return Result<int>.Failure(DomainErrors.MembershipRequest.ApprovalSystemError);
        }

        var persisted = false;
        Result<int>? persistFailure = null;

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var userResult = User.Create(request.Email, request.Email, request.FirstName, request.LastName);
                if (userResult.IsFailure)
                {
                    persistFailure = Result<int>.Failure(userResult.Error);
                    throw new InvalidOperationException(userResult.Error.Code);
                }

                var identityResult = ExternalIdentity.Create(userResult.Value.Id, "keycloak", providerUserId);
                if (identityResult.IsFailure)
                {
                    persistFailure = Result<int>.Failure(identityResult.Error);
                    throw new InvalidOperationException(identityResult.Error.Code);
                }

                _tenantRepository.Add(tenantResult.Value);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var assignmentResult = UserRoleAssignment.Create(
                    userResult.Value.Id,
                    roleResult.Value.Id,
                    tenantResult.Value.Id,
                    branchId: null);

                if (assignmentResult.IsFailure)
                {
                    persistFailure = Result<int>.Failure(assignmentResult.Error);
                    throw new InvalidOperationException(assignmentResult.Error.Code);
                }

                if (!RoleTenantAssignmentRules.IsAllowed(
                        RoleCodes.TenantAdmin,
                        tenantResult.Value.Type,
                        branchId: null))
                {
                    persistFailure = Result<int>.Failure(
                        DomainErrors.UserRoleAssignment.RoleNotAllowedForTenantType);
                    throw new InvalidOperationException(
                        DomainErrors.UserRoleAssignment.RoleNotAllowedForTenantType.Code);
                }

                var approveResult = request.Approve(tenantResult.Value.Id, userResult.Value.Id);
                if (approveResult.IsFailure)
                {
                    persistFailure = Result<int>.Failure(approveResult.Error);
                    throw new InvalidOperationException(approveResult.Error.Code);
                }

                _userRepository.Add(userResult.Value);
                _externalIdentityRepository.Add(identityResult.Value);
                _userRoleAssignmentRepository.Add(assignmentResult.Value);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            persisted = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to persist membership approval for {Email}. Compensating Keycloak user {ProviderUserId}.",
                request.Email,
                providerUserId);

            await CompensateKeycloakUserAsync(providerUserId, cancellationToken);

            return persistFailure ?? Result<int>.Failure(DomainErrors.MembershipRequest.ApprovalSystemError);
        }

        if (!persisted)
        {
            await CompensateKeycloakUserAsync(providerUserId, cancellationToken);
            return Result<int>.Failure(DomainErrors.MembershipRequest.ApprovalSystemError);
        }

        return Result<int>.Success(request.Id);
    }

    private Task CompensateKeycloakUserAsync(string providerUserId, CancellationToken cancellationToken) =>
        _identityAdminService.DeleteUserAsync(providerUserId, cancellationToken);

    private async Task<Result<string>> AllocateTenantCodeAsync(
        string companyName,
        CancellationToken cancellationToken)
    {
        var baseCode = TenantCodeGenerator.FromCompanyName(companyName);
        if (!await _tenantRepository.ExistsByCodeAsync(baseCode, cancellationToken))
        {
            return Result<string>.Success(baseCode);
        }

        for (var suffix = 2; suffix <= 50; suffix++)
        {
            var candidate = $"{baseCode}-{suffix}";
            if (candidate.Length > 64)
            {
                candidate = candidate[..64];
            }

            if (!await _tenantRepository.ExistsByCodeAsync(candidate, cancellationToken))
            {
                return Result<string>.Success(candidate);
            }
        }

        return Result<string>.Failure(DomainErrors.Tenant.CodeGenerationFailed);
    }
}
