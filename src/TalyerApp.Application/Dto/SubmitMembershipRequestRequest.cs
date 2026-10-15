namespace TalyerApp.Application.Dto;

public sealed record SubmitMembershipRequestRequest(
    string CompanyName,
    string Email,
    string FirstName,
    string LastName);

public sealed record RejectMembershipRequestRequest(string? Reason);
