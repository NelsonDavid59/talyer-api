using TalyerApp.Application.Common.Interfaces.CQRS;

namespace TalyerApp.Application.Features.MembershipRequests;

public sealed record RejectMembershipRequestCmd(int Id, string? Reason) : ICommand<int>;
