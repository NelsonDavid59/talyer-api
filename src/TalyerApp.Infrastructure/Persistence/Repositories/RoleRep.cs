using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence.Repositories;

public class RoleRep : IRoleRep
{
    private readonly ApplicationDbContext _context;

    public RoleRep(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Role>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FindAsync(id, cancellationToken);

        if (role is null)
        {
            return Result<Role>.Failure(DomainErrors.Role.RoleNotFound);
        }

        return Result<Role>.Success(role);
    }
}
