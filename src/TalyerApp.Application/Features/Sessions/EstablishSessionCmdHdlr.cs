using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Application.Dto;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.Sessions;

public class EstablishSessionCmdHdlr : ICommandHandler<EstablishSessionCmd, SessionDto>
{
    private readonly IIdentityAdminService _identityAdminService;
    private readonly IExternalIdentityRep _externalIdentityRepository;
    private readonly IUserRep _userRepository;
    private readonly IUserRoleAssignmentRep _userRoleAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDatabaseExceptionTranslator _databaseExceptionTranslator;

    public EstablishSessionCmdHdlr(
        IIdentityAdminService identityAdminService,
        IExternalIdentityRep externalIdentityRepository,
        IUserRep userRepository,
        IUserRoleAssignmentRep userRoleAssignmentRepository,
        IUnitOfWork unitOfWork,
        IDatabaseExceptionTranslator databaseExceptionTranslator)
    {
        _identityAdminService = identityAdminService;
        _externalIdentityRepository = externalIdentityRepository;
        _userRepository = userRepository;
        _userRoleAssignmentRepository = userRoleAssignmentRepository;
        _unitOfWork = unitOfWork;
        _databaseExceptionTranslator = databaseExceptionTranslator;
    }

    public async Task<Result<SessionDto>> HandleAsync(
        EstablishSessionCmd command,
        CancellationToken cancellationToken = default)
    {
        var provider = _identityAdminService.Provider;
        var identityResult = await _externalIdentityRepository
            .GetByProviderAndProviderUserIdAsync(provider, command.ProviderUserId);

        Guid userId;
        if (identityResult.IsSuccess)
        {
            userId = identityResult.Value.UserId;
        }
        else
        {
            var userByEmail = await _userRepository.GetByEmailAsync(command.Email);
            if (userByEmail.IsFailure)
            {
                return Result<SessionDto>.Failure(
                    userByEmail.Error.Code == ErrorCodes.User.NotFound
                        ? DomainErrors.Session.UserNotRegistered
                        : userByEmail.Error);
            }

            var linkResult = await LinkIdentityAsync(
                userByEmail.Value.Id,
                provider,
                command.ProviderUserId,
                cancellationToken);
            if (linkResult.IsFailure)
            {
                return Result<SessionDto>.Failure(linkResult.Error);
            }

            userId = userByEmail.Value.Id;
        }

        var userResult = await _userRepository.GetByIdAsync(userId);
        if (userResult.IsFailure)
        {
            return Result<SessionDto>.Failure(userResult.Error);
        }

        var user = userResult.Value;
        var assignments = await _userRoleAssignmentRepository.GetByUserIdAsync(userId, cancellationToken);

        return Result<SessionDto>.Success(new SessionDto(
            user.Id,
            user.Email,
            user.Username,
            user.FirstName,
            user.LastName,
            assignments.Select(a => new SessionAssignmentDto(
                a.Role.Code,
                a.TenantId,
                a.Tenant.Code,
                a.BranchId)).ToList()));
    }

    private async Task<Result> LinkIdentityAsync(
        Guid userId,
        string provider,
        string providerUserId,
        CancellationToken cancellationToken)
    {
        var externalIdentity = ExternalIdentity.Create(userId, provider, providerUserId);
        if (externalIdentity.IsFailure)
        {
            return Result.Failure(externalIdentity.Error);
        }

        _externalIdentityRepository.Add(externalIdentity.Value);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var translated = _databaseExceptionTranslator.FromSaveChanges(
                ex,
                new DatabaseExceptionContext(DomainErrors.ExternalIdentity.AlreadyExists));

            if (translated is not null)
            {
                return Result.Failure(translated);
            }

            throw;
        }
    }
}
