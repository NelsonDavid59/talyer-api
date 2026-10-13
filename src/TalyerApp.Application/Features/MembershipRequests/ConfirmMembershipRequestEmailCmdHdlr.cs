using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.MembershipRequests;

public class ConfirmMembershipRequestEmailCmdHdlr : ICommandHandler<ConfirmMembershipRequestEmailCmd, int>
{
    private readonly IMembershipRequestRep _membershipRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmMembershipRequestEmailCmdHdlr(
        IMembershipRequestRep membershipRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _membershipRequestRepository = membershipRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> HandleAsync(
        ConfirmMembershipRequestEmailCmd command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Token))
        {
            return Result<int>.Failure(DomainErrors.MembershipRequest.InvalidToken);
        }

        var hash = EmailVerificationToken.Hash(command.Token.Trim());
        var requestResult = await _membershipRequestRepository
            .GetByEmailVerificationTokenHashAsync(hash, cancellationToken);

        if (requestResult.IsFailure)
        {
            return Result<int>.Failure(requestResult.Error);
        }

        var verified = requestResult.Value.MarkEmailVerified();
        if (verified.IsFailure)
        {
            return Result<int>.Failure(verified.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(requestResult.Value.Id);
    }
}
