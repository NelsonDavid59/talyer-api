using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class RolePermission : IBaseEntity
{
    public int RoleId { get; private set; }
    public int PermissionId { get; private set; }

    public Role Role { get; }
    public Permission Permission { get; }

    private RolePermission(int roleId, int permissionid)
    {
        RoleId = roleId;
        PermissionId = permissionid;
    }

    public static Result<RolePermission> Create(int roleId, int permissionId)
    {
        if(roleId < 0)
        {
            return Result<RolePermission>.Failure(DomainErrors.RolePermission.RolePermissionInvalidRoleId);
        }

        if(permissionId < 0)
        {
            return Result<RolePermission>.Failure(DomainErrors.RolePermission.RolePermissionInvalidPermissionId);
        }
        
        return Result<RolePermission>.Success(new RolePermission(roleId, permissionId));
    }
}