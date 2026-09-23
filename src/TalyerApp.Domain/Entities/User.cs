using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class User : BaseEntity
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Username { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public ICollection<ExternalIdentity> ExternalIdentities { get; set; }

    private User(string email, string username, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        Email = email;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<User> Create(string email, string username, string firstName, string lastName)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            return Result<User>.Failure(DomainErrors.User.InvalidUserEmailFormat);
        }
        
        if(string.IsNullOrWhiteSpace(username))
        {
            return Result<User>.Failure(DomainErrors.User.InvalidUserUsernameFormat);
        }

        if(string.IsNullOrWhiteSpace(firstName))
        {
            return Result<User>.Failure(DomainErrors.User.InvalidUserFirstNameFormat);
        }

        if(string.IsNullOrWhiteSpace(lastName))
        {
            return Result<User>.Failure(DomainErrors.User.InvalidUserLastNameFormat);
        }

        return Result<User>.Success(new User(email, username, firstName, lastName));
    }
}