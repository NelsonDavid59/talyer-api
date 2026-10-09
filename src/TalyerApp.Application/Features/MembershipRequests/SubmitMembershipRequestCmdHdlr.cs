using Microsoft.Extensions.Options;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.MembershipRequests;

public class SubmitMembershipRequestCmdHdlr : ICommandHandler<SubmitMembershipRequestCmd, int>
{
    private readonly IMembershipRequestRep _membershipRequestRepository;
    private readonly ITenantRep _tenantRepository;
    private readonly IUserRep _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly MembershipOptions _options;

    public SubmitMembershipRequestCmdHdlr(
        IMembershipRequestRep membershipRequestRepository,
        ITenantRep tenantRepository,
        IUserRep userRepository,
        IUnitOfWork unitOfWork,
        IEmailSender emailSender,
        IOptions<MembershipOptions> options)
    {
        _membershipRequestRepository = membershipRequestRepository;
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _options = options.Value;
    }

    public async Task<Result<int>> HandleAsync(
        SubmitMembershipRequestCmd command,
        CancellationToken cancellationToken = default)
    {
        var email = command.Email.Trim();
        var companyName = command.CompanyName.Trim();

        if (await _membershipRequestRepository.ExistsActiveByEmailAsync(email, cancellationToken) ||
            await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            return Result<int>.Failure(DomainErrors.MembershipRequest.AlreadyExists);
        }

        if (await _tenantRepository.ExistsByDescriptionAsync(companyName, cancellationToken))
        {
            return Result<int>.Failure(DomainErrors.Tenant.AlreadyExists);
        }

        var createResult = MembershipRequest.Create(
            companyName,
            email,
            command.FirstName,
            command.LastName);

        if (createResult.IsFailure)
        {
            return Result<int>.Failure(createResult.Error);
        }

        var (request, plainToken) = createResult.Value;
        _membershipRequestRepository.Add(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var confirmUrl = $"{_options.ConfirmEmailBaseUrl.TrimEnd('/')}/membership-requests/confirm-email";
        var body =
            $"Confirm your email by POSTing JSON {{\"token\":\"{plainToken}\"}} to {confirmUrl}. Token: {plainToken}";

        await _emailSender.SendAsync(
            request.Email,
            "Confirm your Talyer membership request",
            body,
            cancellationToken);

        return Result<int>.Success(request.Id);
    }
}
