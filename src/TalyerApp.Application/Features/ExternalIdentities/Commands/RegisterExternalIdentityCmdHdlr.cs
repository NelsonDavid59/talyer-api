

using TalyerApp.Application.Common.Interfaces.CQRS;
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

    public RegisterExternalIdentityCmdHdlr(
        IUserRep userRepository,
        IExternalIdentityRep externalIdentityRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _externalIdentityRepository = externalIdentityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(
        RegisterExternalIdentityCmd command, 
        CancellationToken cancellationToken = default)
    {
        // Validates if the external identity already exists
        var existingExternalIdentity = 
            await _externalIdentityRepository
                .GetByProviderAndProviderUserIdAsync(command.Provider, command.ProviderUserId);

        if(existingExternalIdentity.IsSuccess)
        {
            return Result<Guid>.Success(existingExternalIdentity.Value.UserId);
        }

        // Create a User
        var user = User.Create(command.Email, command.Username, command.FirstName, command.LastName);

        if (user.IsFailure)
        {
            return Result<Guid>.Failure(user.Error);
        }

        // Create an ExternalIdentity
        var externalIdentity = ExternalIdentity.Create(user.Value.Id, command.Provider, command.ProviderUserId);

        if (externalIdentity.IsFailure)
        {
            return Result<Guid>.Failure(externalIdentity.Error);
        }

        // Add the User and ExternalIdentity to the repositories
        _userRepository.Add(user.Value);
        _externalIdentityRepository.Add(externalIdentity.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Value.Id);
    }
}