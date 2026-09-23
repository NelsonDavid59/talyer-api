using TalyerApp.Application.Common.Interfaces.CQRS;

namespace TalyerApp.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string Username,
    string FirstName, 
    string LastName, 
    string Email) : ICommand<Guid>;