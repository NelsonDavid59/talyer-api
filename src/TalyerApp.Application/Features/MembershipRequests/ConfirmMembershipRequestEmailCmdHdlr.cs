using Microsoft.Extensions.Logging;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.MembershipRequests;

public class ConfirmMembershipRequestEmailCmdHdlr : ICommandHandler<ConfirmMembershipRequestEmailCmd, int>
{
    private readonly IMembershipRequestRep _membershipRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMembershipEmailService _membershipEmailService;
    private readonly ILogger<ConfirmMembershipRequestEmailCmdHdlr> _logger;

    public ConfirmMembershipRequestEmailCmdHdlr(
        IMembershipRequestRep membershipRequestRepository,
        IUnitOfWork unitOfWork,
        IMembershipEmailService membershipEmailService,
        ILogger<ConfirmMembershipRequestEmailCmdHdlr> logger)
    {
        _membershipRequestRepository = membershipRequestRepository;
        _unitOfWork = unitOfWork;
        _membershipEmailService = membershipEmailService;
        _logger = logger;
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

        var request = requestResult.Value;
        Result<int>? failure = null;
        var confirmationEmailSent = false;

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var verified = request.MarkEmailVerified();
                if (verified.IsFailure)
                {
                    failure = Result<int>.Failure(verified.Error);
                    throw new InvalidOperationException(verified.Error.Code);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var emailResult = await _membershipEmailService.SendEmailConfirmedAsync(
                    request,
                    cancellationToken);

                if (emailResult.IsFailure)
                {
                    _logger.LogError(
                        "Membership request {MembershipRequestId} confirmation rolled back: success email failed for {Email}. Request remains PendingEmailVerification.",
                        request.Id,
                        request.Email);
                    failure = Result<int>.Failure(DomainErrors.MembershipRequest.ConfirmationEmailFailed);
                    throw new InvalidOperationException(emailResult.Error.Code);
                }

                confirmationEmailSent = true;
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            if (confirmationEmailSent)
            {
                _logger.LogCritical(
                    ex,
                    "Membership request {MembershipRequestId} success email was sent but confirmation was not committed for {Email}.",
                    request.Id,
                    request.Email);
            }
            else if (failure is null)
            {
                _logger.LogError(
                    ex,
                    "Membership request {MembershipRequestId} confirmation failed for {Email}.",
                    request.Id,
                    request.Email);
            }

            return failure ?? Result<int>.Failure(DomainErrors.MembershipRequest.ConfirmationEmailFailed);
        }

        return Result<int>.Success(request.Id);
    }
}
