using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.CQRS;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default);
}    