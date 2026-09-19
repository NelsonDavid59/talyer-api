using TalyerApp.Domain.Entities;

namespace TalyerApp.Application.Dto;

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static UserProfileDto FromEntity(UserProfile userProfile)
    {
        return new UserProfileDto
        {
            UserId = userProfile.UserId,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            CreatedAt = userProfile.CreatedAt,
            UpdatedAt = userProfile.UpdatedAt
        };
    }
}

