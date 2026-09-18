using TalyerApp.Domain.Shared;

namespace TalyerApp.Domain.Entities;

public class UserProfile
{
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UserProfile(string firstName, string lastName)
    {
        UserId = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Result<UserProfile> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<UserProfile>.Failure(DomainErrors.UserProfile.InvalidUserProfileFirstNameFormat);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<UserProfile>.Failure(DomainErrors.UserProfile.InvalidUserProfileLastNameFormat);

        return Result<UserProfile>.Success(new UserProfile(firstName, lastName));
    }
}