using TalyerApp.Domain.Entities;

namespace TalyerApp.Application.Dto;

public class MembershipRequestDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? RejectedReason { get; set; }
    public int? TenantId { get; set; }
    public Guid? UserId { get; set; }

    public static MembershipRequestDto FromEntity(MembershipRequest request) =>
        new()
        {
            Id = request.Id,
            CompanyName = request.CompanyName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Status = request.Status.ToString(),
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            RejectedReason = request.RejectedReason,
            TenantId = request.TenantId,
            UserId = request.UserId
        };
}
