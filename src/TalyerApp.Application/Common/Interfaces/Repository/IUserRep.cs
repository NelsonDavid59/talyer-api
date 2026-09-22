using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface IUserRep : IBaseRepository<User>
{
    public Task<Result<User>> GetByIdAsync(Guid id);
    public Task<Result<User>> GetByEmailAsync(string email);
    public void Add(User user);
}