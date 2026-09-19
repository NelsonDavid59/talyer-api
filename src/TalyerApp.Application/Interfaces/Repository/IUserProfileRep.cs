using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;
using TalyerApp.Application.Dto;

namespace TalyerApp.Application.Interfaces.Repository;

public interface IUserProfileRep : IBaseRepository<UserProfile>
{
    Task<Result<UserProfile>> GetByUserIdAsync(Guid userId);
}