using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Interfaces;

public interface ICommandDispatcher
{
    Task<Result> DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    Task<Result<TResponse>> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>;
}


public interface IQueryDispatcher
{
    Task<Result<TResponse>> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>;
}