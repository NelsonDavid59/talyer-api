using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface IBaseRepository<T> where T : IBaseEntity
{
    Task<Result<IEnumerable<T>>> GetAllAsync();
}