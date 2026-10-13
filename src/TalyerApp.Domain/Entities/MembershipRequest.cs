using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class MembershipRequest : BaseEntity
{
    public int Id { get; private set; }
    public string CompanyName { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public MembershipRequestStatus Status { get; private set; }
    public string EmailVerificationTokenHash { get; private set; }
    public DateTime EmailVerificationExpiresAt { get; private set; }
    public int? TenantId { get; private set; }
    public Guid? UserId { get; private set; }
    public string? RejectedReason { get; private set; }

    private MembershipRequest(
        string companyName,
        string email,
        string firstName,
        string lastName,
        string emailVerificationTokenHash,
        DateTime emailVerificationExpiresAt)
    {
        CompanyName = companyName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Status = MembershipRequestStatus.PendingEmailVerification;
        EmailVerificationTokenHash = emailVerificationTokenHash;
        EmailVerificationExpiresAt = emailVerificationExpiresAt;
    }

    public static Result<(MembershipRequest Request, string PlainToken)> Create(
        string companyName,
        string email,
        string firstName,
        string lastName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return Result<(MembershipRequest, string)>.Failure(DomainErrors.MembershipRequest.InvalidCompanyName);
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<(MembershipRequest, string)>.Failure(DomainErrors.MembershipRequest.InvalidEmail);
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<(MembershipRequest, string)>.Failure(DomainErrors.MembershipRequest.InvalidFirstName);
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<(MembershipRequest, string)>.Failure(DomainErrors.MembershipRequest.InvalidLastName);
        }

        var plainToken = EmailVerificationToken.GeneratePlaintext();
        var request = new MembershipRequest(
            companyName.Trim(),
            email.Trim(),
            firstName.Trim(),
            lastName.Trim(),
            EmailVerificationToken.Hash(plainToken),
            DateTime.UtcNow.AddHours(EmailVerificationConstants.LifetimeHours));

        return Result<(MembershipRequest, string)>.Success((request, plainToken));
    }

    public Result MarkEmailVerified()
    {
        if (Status != MembershipRequestStatus.PendingEmailVerification)
        {
            return Result.Failure(DomainErrors.MembershipRequest.InvalidStatus);
        }

        if (DateTime.UtcNow > EmailVerificationExpiresAt)
        {
            return Result.Failure(DomainErrors.MembershipRequest.TokenExpired);
        }

        Status = MembershipRequestStatus.PendingReview;
        UpdateTimestamp();
        return Result.Success();
    }

    public Result Approve(int tenantId, Guid userId)
    {
        if (Status != MembershipRequestStatus.PendingReview)
        {
            return Result.Failure(DomainErrors.MembershipRequest.InvalidStatus);
        }

        if (tenantId <= 0)
        {
            return Result.Failure(DomainErrors.UserRoleAssignment.InvalidTenantId);
        }

        if (userId == Guid.Empty)
        {
            return Result.Failure(DomainErrors.UserRoleAssignment.InvalidUserId);
        }

        Status = MembershipRequestStatus.Approved;
        TenantId = tenantId;
        UserId = userId;
        UpdateTimestamp();
        return Result.Success();
    }

    public Result Reject(string reason)
    {
        if (Status != MembershipRequestStatus.PendingReview &&
            Status != MembershipRequestStatus.PendingEmailVerification)
        {
            return Result.Failure(DomainErrors.MembershipRequest.InvalidStatus);
        }

        Status = MembershipRequestStatus.Rejected;
        RejectedReason = string.IsNullOrWhiteSpace(reason) ? "Rejected" : reason.Trim();
        UpdateTimestamp();
        return Result.Success();
    }
}
