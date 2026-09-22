using Microsoft.Extensions.DependencyInjection;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Interfaces;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Dispatching;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    Task<Result<TResponse>> IQueryDispatcher.DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
    {
        var handler = _serviceProvider
                        .GetRequiredService<IQueryHandler<TQuery, TResponse>>();
                        
        return handler.HandleAsync(query, cancellationToken);     
    }
}