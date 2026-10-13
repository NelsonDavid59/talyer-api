using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface IExternalIdentityRep : IBaseRepository<ExternalIdentity>
{
    public Task<Result<ExternalIdentity>> GetByProviderAndProviderUserIdAsync(string provider, string providerUserId);
    public void Add(ExternalIdentity externalIdentity);
    public Task<bool> ExistsAsync(string provider, string providerUserId);
}