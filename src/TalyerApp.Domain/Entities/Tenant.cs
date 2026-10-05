using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class Tenant : BaseEntity
{
    public int Id { get; }
    public string Description { get; private set; }

    private Tenant(string description)
    {
        Description = description;
    }

    public static Result<Tenant> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<Tenant>.Failure(DomainErrors.Tenant.TenantInvalidDescription);
        }

        return Result<Tenant>.Success(new Tenant(description));
    }
}