using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.ExternalIdentities;

public class RegisterExternalIdentityCmdHdlr
    : ICommandHandler<RegisterExternalIdentityCmd, Guid>
{
    private readonly IUserRep _userRepository;
    private readonly IExternalIdentityRep _externalIdentityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDatabaseExceptionTranslator _databaseExceptionTranslator;

    public RegisterExternalIdentityCmdHdlr(
        IUserRep userRepository,
        IExternalIdentityRep externalIdentityRepository,
        IUnitOfWork unitOfWork,
        IDatabaseExceptionTranslator databaseExceptionTranslator)
    {
        _userRepository = userRepository;
        _externalIdentityRepository = externalIdentityRepository;
        _unitOfWork = unitOfWork;
        _databaseExceptionTranslator = databaseExceptionTranslator;
    }

    public async Task<Result<Guid>> HandleAsync(
        RegisterExternalIdentityCmd command,
        CancellationToken cancellationToken = default)
    {
        var existingExternalIdentity =
            await _externalIdentityRepository
                .GetByProviderAndProviderUserIdAsync(command.Provider, command.ProviderUserId);

        if (existingExternalIdentity.IsSuccess)
        {
            return Result<Guid>.Success(existingExternalIdentity.Value.UserId);
        }

        var existingUser = await _userRepository.GetByEmailAsync(command.Email);
        if (existingUser.IsSuccess)
        {
            return await LinkIdentityAsync(
                existingUser.Value.Id,
                command,
                cancellationToken);
        }

        if (existingUser.Error.Code != ErrorCodes.User.NotFound)
        {
            return Result<Guid>.Failure(existingUser.Error);
        }

        var user = User.Create(command.Email, command.Username, command.FirstName, command.LastName);
        if (user.IsFailure)
        {
            return Result<Guid>.Failure(user.Error);
        }

        var externalIdentity = ExternalIdentity.Create(user.Value.Id, command.Provider, command.ProviderUserId);
        if (externalIdentity.IsFailure)
        {
            return Result<Guid>.Failure(externalIdentity.Error);
        }

        _userRepository.Add(user.Value);
        _externalIdentityRepository.Add(externalIdentity.Value);

        var persistResult = await PersistAsync(cancellationToken);
        if (persistResult.IsFailure)
        {
            return Result<Guid>.Failure(persistResult.Error);
        }

        return Result<Guid>.Success(user.Value.Id);
    }

    private async Task<Result<Guid>> LinkIdentityAsync(
        Guid userId,
        RegisterExternalIdentityCmd command,
        CancellationToken cancellationToken)
    {
        var externalIdentity = ExternalIdentity.Create(userId, command.Provider, command.ProviderUserId);
        if (externalIdentity.IsFailure)
        {
            return Result<Guid>.Failure(externalIdentity.Error);
        }

        _externalIdentityRepository.Add(externalIdentity.Value);

        var persistResult = await PersistAsync(cancellationToken);
        if (persistResult.IsFailure)
        {
            return Result<Guid>.Failure(persistResult.Error);
        }

        return Result<Guid>.Success(userId);
    }

    private async Task<Result> PersistAsync(CancellationToken cancellationToken)
    {
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
