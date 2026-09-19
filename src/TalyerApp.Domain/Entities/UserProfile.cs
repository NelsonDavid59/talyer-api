using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    private UserProfile(string firstName, string lastName)
    {
        UserId = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
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