using Microsoft.EntityFrameworkCore;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Repositories;

public class UserRoleAssignmentRep : IUserRoleAssignmentRep
{
    private readonly ApplicationDbContext _context;

    public UserRoleAssignmentRep(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        Guid userId,
        int roleId,
        int tenantId,
        int? branchId,
        CancellationToken cancellationToken = default)
    {
        if (branchId is null)
        {
            return await _context.UserRoleAssignments.AnyAsync(
                a => a.UserId == userId
                    && a.RoleId == roleId
                    && a.TenantId == tenantId
                    && a.BranchId == null,
                cancellationToken);
        }

        return await _context.UserRoleAssignments.AnyAsync(
            a => a.UserId == userId
                && a.RoleId == roleId
                && a.TenantId == tenantId
                && a.BranchId == branchId,
            cancellationToken);
    }

    public void Add(UserRoleAssignment assignment)
    {
        _context.UserRoleAssignments.Add(assignment);
    }
}
