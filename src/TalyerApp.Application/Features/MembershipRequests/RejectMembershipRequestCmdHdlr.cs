using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.MembershipRequests;

public class RejectMembershipRequestCmdHdlr : ICommandHandler<RejectMembershipRequestCmd, int>
{
    private readonly IMembershipRequestRep _membershipRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectMembershipRequestCmdHdlr(
        IMembershipRequestRep membershipRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _membershipRequestRepository = membershipRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> HandleAsync(
        RejectMembershipRequestCmd command,
        CancellationToken cancellationToken = default)
    {
        var requestResult = await _membershipRequestRepository.GetByIdAsync(command.Id, cancellationToken);
        if (requestResult.IsFailure)
        {
            return Result<int>.Failure(requestResult.Error);
        }

        var rejected = requestResult.Value.Reject(command.Reason ?? "Rejected");
        if (rejected.IsFailure)
        {
            return Result<int>.Failure(rejected.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(requestResult.Value.Id);
    }
}
