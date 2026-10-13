using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Interfaces;
using TalyerApp.Domain.Shared.Result;
using Microsoft.Extensions.DependencyInjection;

namespace TalyerApp.Application.Common.Dispatching;

public class CommandDispatcher : ICommandDispatcher
{
    // The service provider is used to resolve the command handlers at runtime.
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<Result> DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        var handler = _serviceProvider
                        .GetRequiredService<ICommandHandler<TCommand>>();
        return handler.HandleAsync(command, cancellationToken);
    }

    public Task<Result<TResponse>> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TResponse>
    {
        var handler = _serviceProvider
                        .GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        return handler.HandleAsync(command, cancellationToken);
    }
}