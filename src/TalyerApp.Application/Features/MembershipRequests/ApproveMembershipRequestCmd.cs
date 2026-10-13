using TalyerApp.Application.Common.Interfaces.CQRS;

namespace TalyerApp.Application.Features.MembershipRequests;

public sealed record ApproveMembershipRequestCmd(int Id) : ICommand<int>;
