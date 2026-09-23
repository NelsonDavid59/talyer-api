using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.Users.Commands;

public class CreateUserCmdHdlr : ICommandHandler<CreateUserCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}