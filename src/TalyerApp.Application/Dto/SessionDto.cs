namespace TalyerApp.Application.Dto;

public sealed record SessionAssignmentDto(
    string RoleCode,
    int TenantId,
    string TenantCode,
    int? BranchId);

public sealed record SessionDto(
    Guid UserId,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    IReadOnlyList<SessionAssignmentDto> Assignments);
