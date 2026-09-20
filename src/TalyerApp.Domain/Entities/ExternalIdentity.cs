using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class ExternalIdentity : BaseEntity
{
    public Guid UserId { get; private set; }

    public string Provider { get; private set; }

    public string ProviderUserId { get; private set; }

    private ExternalIdentity(Guid userId, string provider, string providerUserId)
    {
        UserId = userId;
        Provider = provider;
        ProviderUserId = providerUserId;
    }

    public static Result<ExternalIdentity> Create(Guid userId, string provider, string providerUserId)
    {
        if(string.IsNullOrWhiteSpace(provider))
        {
            return Result<ExternalIdentity>.Failure(DomainErrors.ExternalIdentity.InvalidExternalIdentityProvider);
        }

        if(string.IsNullOrWhiteSpace(providerUserId))
        {
            return Result<ExternalIdentity>.Failure(DomainErrors.ExternalIdentity.InvalidExternalIdentityProviderUserId);
        }

        var externalIdentity = new ExternalIdentity(userId, provider, providerUserId);
        return Result<ExternalIdentity>.Success(externalIdentity);
    }
}