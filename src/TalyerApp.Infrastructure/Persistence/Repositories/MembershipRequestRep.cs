using Microsoft.EntityFrameworkCore;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence.Repositories;

public class MembershipRequestRep : IMembershipRequestRep
{
    private readonly ApplicationDbContext _context;

    public MembershipRequestRep(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MembershipRequest>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var request = await _context.MembershipRequests.FindAsync([id], cancellationToken);
        if (request is null)
        {
            return Result<MembershipRequest>.Failure(DomainErrors.MembershipRequest.NotFound);
        }

        return Result<MembershipRequest>.Success(request);
    }

    public async Task<Result<MembershipRequest>> GetByEmailVerificationTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        var request = await _context.MembershipRequests
            .FirstOrDefaultAsync(r => r.EmailVerificationTokenHash == tokenHash, cancellationToken);

        if (request is null)
        {
            return Result<MembershipRequest>.Failure(DomainErrors.MembershipRequest.InvalidToken);
        }

        return Result<MembershipRequest>.Success(request);
    }

    public async Task<IReadOnlyList<MembershipRequest>> ListByStatusAsync(
        MembershipRequestStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.MembershipRequests
            .AsNoTracking()
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsActiveByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.MembershipRequests.AnyAsync(
            r => r.Email == email &&
                 (r.Status == MembershipRequestStatus.PendingEmailVerification ||
                  r.Status == MembershipRequestStatus.PendingReview),
            cancellationToken);
    }

    public void Add(MembershipRequest request)
    {
        _context.MembershipRequests.Add(request);
    }
}
