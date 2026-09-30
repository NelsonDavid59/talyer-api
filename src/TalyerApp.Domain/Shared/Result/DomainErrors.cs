namespace TalyerApp.Domain.Shared.Result;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error UserNotFound = 
            Error.NotFound("User.NotFound", "User not found.");
        
        public static readonly Error InvalidUserEmailFormat = 
            Error.Validation("User.InvalidEmail", "User email format is invalid.");

        public static readonly Error InvalidUserUsernameFormat = 
            Error.Validation("User.InvalidUsername", "User username format is invalid.");

        public static readonly Error InvalidUserFirstNameFormat = 
            Error.Validation("User.InvalidFirstName", "User first name format is invalid.");

        public static readonly Error InvalidUserLastNameFormat = 
            Error.Validation("User.InvalidLastName", "User last name format is invalid.");
    }

    public static class ExternalIdentity
    {

        public static readonly Error ExternalIdentityNotFound = 
            Error.NotFound("ExternalIdentity.NotFound", "External identity not found.");
        public static readonly Error InvalidExternalIdentityProvider = 
            Error.Validation("ExternalIdentity.InvalidProvider", "ExternalIdentity Provider is invalid.");

        public static readonly Error InvalidExternalIdentityProviderUserId = 
            Error.Validation("ExternalIdentity.InvalidProviderUserId", "ExternalIdentity ProviderUserId is invalid.");
    }

    public static class Role
    {
        public static readonly Error RoleNotFound = Error.NotFound("Role.NotFound", "Role not found");
        public static readonly Error InvalidRoleName = Error.Validation("Role.InvalidRoleName", "Role RoleName is invalid");
    }

    public static class Permission
    {
        public static readonly Error PermissionNotFound = Error.NotFound("Permission.NotFound", "Permission not found.");
        public static readonly Error PermissionInvalidCode = Error.Validation("Permission.InvalidCode", "Permission Code is invalid.");
    }

    public static class RolePermission
    {
        public static readonly Error RolePermissionNotFound = 
            Error.NotFound("RolePermission.NotFound", "RolePermission not found.");
        public static readonly Error RolePermissionInvalidRoleId =
            Error.Validation("RolePermission.InvalidRoleId", "RolePermission RoleId is invalid.");
        public static readonly Error RolePermissionInvalidPermissionId =
            Error.Validation("RolePermission.InvalidPermissionId", "RolePermission PermissionId is invalid");
    }
}