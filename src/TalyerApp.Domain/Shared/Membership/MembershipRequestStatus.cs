namespace TalyerApp.Domain.Shared.Membership;

public enum MembershipRequestStatus
{
    PendingEmailVerification,
    PendingReview,
    Approved,
    Rejected
}
