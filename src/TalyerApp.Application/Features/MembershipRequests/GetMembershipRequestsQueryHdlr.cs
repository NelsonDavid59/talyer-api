using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Application.Dto;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.MembershipRequests;

public class GetMembershipRequestsQueryHdlr
    : IQueryHandler<GetMembershipRequestsQuery, IReadOnlyList<MembershipRequestDto>>
{
    private readonly IMembershipRequestRep _membershipRequestRepository;

    public GetMembershipRequestsQueryHdlr(IMembershipRequestRep membershipRequestRepository)
    {
        _membershipRequestRepository = membershipRequestRepository;
    }

    public async Task<Result<IReadOnlyList<MembershipRequestDto>>> HandleAsync(
        GetMembershipRequestsQuery query,
        CancellationToken cancellationToken = default)
    {
        var requests = await _membershipRequestRepository.ListByStatusAsync(query.Status, cancellationToken);
        var dtos = requests.Select(MembershipRequestDto.FromEntity).ToList();
        return Result<IReadOnlyList<MembershipRequestDto>>.Success(dtos);
    }
}
