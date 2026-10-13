using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class Branch : BaseEntity
{
    public int Id { get; }
    public int TenantId { get; }
    public string Description { get; private set; }

    private Branch(string description, int tenantId)
    {
        Description = description;
        TenantId = tenantId;
    }

    public static Result<Branch> Create(string description, int tenantId)
    {
        if (tenantId <= 0)
        {
            return Result<Branch>.Failure(DomainErrors.Branch.BranchInvalidTenantId);
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<Branch>.Failure(DomainErrors.Branch.BranchInvalidDescription);
        }

        return Result<Branch>.Success(new Branch(description, tenantId));
    }
}