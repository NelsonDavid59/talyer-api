using TalyerApp.Domain.Shared;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class Role : IBaseEntity
{
    public int Id { get; }
    public string RoleName { get; private set; }

    private Role(string roleName)
    {
        RoleName = roleName;
    }

    public static Result<Role> Create(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return Result<Role>.Failure(DomainErrors.Role.InvalidRoleName);
        }

        return Result<Role>.Success(new Role(roleName));
    }
}