using TalyerApp.Domain.Shared.Result;
using TalyerApp.Domain.Shared.Tenancy;

namespace TalyerApp.Domain.Entities;

public class Tenant : BaseEntity
{
    public int Id { get; }
    public string Description { get; private set; }
    public string Code { get; private set; }
    public TenantType Type { get; private set; }

    private Tenant(string description, string code, TenantType type)
    {
        Description = description;
        Code = code;
        Type = type;
    }

    public static Result<Tenant> CreatePlatform(string description, string code) =>
        Create(description, code, TenantType.Platform);

    public static Result<Tenant> CreateCustomer(string description, string code) =>
        Create(description, code, TenantType.Customer);

    private static Result<Tenant> Create(string description, string code, TenantType type)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<Tenant>.Failure(DomainErrors.Tenant.TenantInvalidDescription);
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<Tenant>.Failure(DomainErrors.Tenant.TenantInvalidCode);
        }

        return Result<Tenant>.Success(new Tenant(description, code, type));
    }
}
