namespace TalyerApp.Application.Dto;

public sealed record SubmitMembershipRequestRequest(
    string CompanyName,
    string Email,
    string FirstName,
    string LastName);

public sealed record ConfirmMembershipRequestEmailRequest(string Token);

public sealed record RejectMembershipRequestRequest(string? Reason);
