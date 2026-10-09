using TalyerApp.Application.Common.Interfaces.CQRS;

namespace TalyerApp.Application.Features.MembershipRequests;

public sealed record SubmitMembershipRequestCmd(
    string CompanyName,
    string Email,
    string FirstName,
    string LastName
) : ICommand<int>;
