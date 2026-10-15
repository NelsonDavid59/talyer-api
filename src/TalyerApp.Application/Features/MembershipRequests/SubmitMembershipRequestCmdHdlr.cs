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
    private readonly IMembershipEmailService _membershipEmailService;

    public SubmitMembershipRequestCmdHdlr(
        IMembershipRequestRep membershipRequestRepository,
        ITenantRep tenantRepository,
        IUserRep userRepository,
        IUnitOfWork unitOfWork,
        IMembershipEmailService membershipEmailService)
    {
        _membershipRequestRepository = membershipRequestRepository;
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _membershipEmailService = membershipEmailService;
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

        var emailResult = await _membershipEmailService.SendConfirmRequestAsync(
            request,
            plainToken,
            cancellationToken);

        if (emailResult.IsFailure)
        {
            return Result<int>.Failure(emailResult.Error);
        }

        return Result<int>.Success(request.Id);
    }
}
