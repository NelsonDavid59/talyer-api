using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Dto;
using TalyerApp.Domain.Shared.Membership;

namespace TalyerApp.Application.Features.MembershipRequests;

public sealed record GetMembershipRequestsQuery(MembershipRequestStatus Status)
    : IQuery<IReadOnlyList<MembershipRequestDto>>;
