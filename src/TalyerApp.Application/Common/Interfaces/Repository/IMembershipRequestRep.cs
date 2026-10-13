using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface IMembershipRequestRep
{
    Task<Result<MembershipRequest>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<MembershipRequest>> GetByEmailVerificationTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MembershipRequest>> ListByStatusAsync(
        MembershipRequestStatus status,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsActiveByEmailAsync(string email, CancellationToken cancellationToken = default);

    void Add(MembershipRequest request);
}
