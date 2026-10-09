using TalyerApp.Application.Common.Interfaces.CQRS;

namespace TalyerApp.Application.Features.MembershipRequests;

public sealed record ConfirmMembershipRequestEmailCmd(string Token) : ICommand<int>;
