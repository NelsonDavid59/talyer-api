using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Domain.Entities;

public class Permission : IBaseEntity
{
    public int Id { get; }
    public string Code { get; private set; }
    
    private Permission(string code)
    {
        Code = code;
    }

    public static Result<Permission> Create(string code)
    {
        if(string.IsNullOrWhiteSpace(code)) 
            return Result<Permission>.Failure(DomainErrors.Permission.PermissionInvalidCode);

        return Result<Permission>.Success(new Permission(code));
    }
}