using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class Role : BaseEntity
{
    public int Id { get; }
    public string RoleName { get; private set; }

    public string Code { get; private set; }
    
    private Role(string roleName, string code)
    {
        RoleName = roleName;
        Code = code;
    }
    
    public static Result<Role> Create(string roleName, string code)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return Result<Role>.Failure(DomainErrors.Role.InvalidRoleName);
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<Role>.Failure(DomainErrors.Role.InvalidRoleCode);
        }

        return Result<Role>.Success(new Role(roleName, code));
    }
}