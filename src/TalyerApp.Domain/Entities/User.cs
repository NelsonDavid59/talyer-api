using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class User : BaseEntity
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Username { get; private set; }
    public string Name { get; private set; }
    public string LastName { get; private set; }

    private User(string email, string username, string name, string lastName)
    {
        Id = Guid.NewGuid();
        Email = email;
        Username = username;
        Name = name;
        LastName = lastName;
    }

    public static Result<User> Create(string email, string username, string name, string lastName)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            return Result<User>.Failure(DomainErrors.User.InvalidUserEmailFormat);
        }
        
        if(string.IsNullOrWhiteSpace(username))
        {
            return Result<User>.Failure(DomainErrors.User.InvalidUserUsernameFormat);
        }

        if(string.IsNullOrWhiteSpace(name))
        {
            return Result<User>.Failure(DomainErrors.User.InvalidUserNameFormat);
        }

        if(string.IsNullOrWhiteSpace(lastName))
        {
            return Result<User>.Failure(DomainErrors.User.InvalidUserLastNameFormat);
        }

        return Result<User>.Success(new User(email, username, name, lastName));
    }
}