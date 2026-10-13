using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Seeding;

public class RbacReferenceDataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RbacReferenceDataSeeder> _logger;

    public RbacReferenceDataSeeder(
        ApplicationDbContext context,
        ILogger<RbacReferenceDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var permissionsInserted = await SeedPermissionsAsync(cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var rolesInserted = await SeedRolesAsync(cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var rolePermissionsInserted = await SeedRolePermissionsAsync(cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "RBAC reference data seed completed. Permissions inserted: {PermissionsInserted}, roles inserted: {RolesInserted}, role permissions inserted: {RolePermissionsInserted}",
            permissionsInserted,
            rolesInserted,
            rolePermissionsInserted);
    }

    private async Task<int> SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        var inserted = 0;

        foreach (var definition in RbacCatalog.Permissions)
        {
            var exists = await _context.Permissions
                .AnyAsync(p => p.Code == definition.Code, cancellationToken);

            if (exists)
            {
                _logger.LogDebug("Permission '{Code}' already exists.", definition.Code);
                continue;
            }

            var result = Permission.Create(definition.Code);
            if (result.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Invalid permission in catalog: '{definition.Code}' ({result.Error.Code}).");
            }

            _context.Permissions.Add(result.Value);
            inserted++;
            _logger.LogInformation("Inserted permission '{Code}'.", definition.Code);
        }

        return inserted;
    }

    private async Task<int> SeedRolesAsync(CancellationToken cancellationToken)
    {
        var inserted = 0;

        foreach (var definition in RbacCatalog.Roles)
        {
            var exists = await _context.Roles
                .AnyAsync(r => r.Code == definition.Code, cancellationToken);

            if (exists)
            {
                _logger.LogDebug("Role '{Code}' already exists.", definition.Code);
                continue;
            }

            var result = Role.Create(definition.RoleName, definition.Code);
            if (result.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Invalid role in catalog: '{definition.Code}' ({result.Error.Code}).");
            }

            _context.Roles.Add(result.Value);
            inserted++;
            _logger.LogInformation("Inserted role '{Code}'.", definition.Code);
        }

        return inserted;
    }

    private async Task<int> SeedRolePermissionsAsync(CancellationToken cancellationToken)
    {
        var roleIdsByCode = await _context.Roles
            .AsNoTracking()
            .ToDictionaryAsync(r => r.Code, r => r.Id, cancellationToken);

        var permissionIdsByCode = await _context.Permissions
            .AsNoTracking()
            .ToDictionaryAsync(p => p.Code, p => p.Id, cancellationToken);

        var inserted = 0;

        foreach (var (roleCode, permissionCodes) in RbacCatalog.RolePermissions)
        {
            if (!roleIdsByCode.TryGetValue(roleCode, out var roleId))
            {
                throw new InvalidOperationException(
                    $"Role '{roleCode}' from catalog is missing in the database after seed.");
            }

            foreach (var permissionCode in permissionCodes)
            {
                if (!permissionIdsByCode.TryGetValue(permissionCode, out var permissionId))
                {
                    throw new InvalidOperationException(
                        $"Permission '{permissionCode}' referenced by role '{roleCode}' is missing in the database.");
                }

                var exists = await _context.RolePermissions
                    .AnyAsync(
                        rp => rp.RoleId == roleId && rp.PermissionId == permissionId,
                        cancellationToken);

                if (exists)
                {
                    _logger.LogDebug(
                        "Role permission '{RoleCode}' -> '{PermissionCode}' already exists.",
                        roleCode,
                        permissionCode);
                    continue;
                }

                var result = RolePermission.Create(roleId, permissionId);
                if (result.IsFailure)
                {
                    throw new InvalidOperationException(
                        $"Invalid role permission '{roleCode}' -> '{permissionCode}' ({result.Error.Code}).");
                }

                _context.RolePermissions.Add(result.Value);
                inserted++;
                _logger.LogInformation(
                    "Inserted role permission '{RoleCode}' -> '{PermissionCode}'.",
                    roleCode,
                    permissionCode);
            }
        }

        return inserted;
    }
}
