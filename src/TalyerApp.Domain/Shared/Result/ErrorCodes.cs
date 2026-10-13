namespace TalyerApp.Domain.Shared.Result;

public static class ErrorCodes
{
    public static class Common
    {
        public const string Unknown = "Errors.Unknown";
        public const string CatalogEntryMissing = "Errors.CatalogEntryMissing";
    }

    public static class User
    {
        public const string NotFound = "User.NotFound";
        public const string InvalidEmail = "User.InvalidEmail";
        public const string InvalidUsername = "User.InvalidUsername";
        public const string InvalidFirstName = "User.InvalidFirstName";
        public const string InvalidLastName = "User.InvalidLastName";
        public const string AlreadyExists = "User.AlreadyExists";
    }

    public static class ExternalIdentity
    {
        public const string NotFound = "ExternalIdentity.NotFound";
        public const string InvalidProvider = "ExternalIdentity.InvalidProvider";
        public const string InvalidProviderUserId = "ExternalIdentity.InvalidProviderUserId";
        public const string AlreadyExists = "ExternalIdentity.AlreadyExists";
    }

    public static class Role
    {
        public const string NotFound = "Role.NotFound";
        public const string InvalidRoleName = "Role.InvalidRoleName";
        public const string InvalidCode = "Role.InvalidCode";
    }

    public static class Permission
    {
        public const string NotFound = "Permission.NotFound";
        public const string InvalidCode = "Permission.InvalidCode";
    }

    public static class RolePermission
    {
        public const string NotFound = "RolePermission.NotFound";
        public const string InvalidRoleId = "RolePermission.InvalidRoleId";
        public const string InvalidPermissionId = "RolePermission.InvalidPermissionId";
        public const string AlreadyExists = "RolePermission.AlreadyExists";
    }

    public static class Tenant
    {
        public const string NotFound = "Tenant.NotFound";
        public const string InvalidDescription = "Tenant.InvalidDescription";
        public const string InvalidCode = "Tenant.InvalidCode";
        public const string AlreadyExists = "Tenant.AlreadyExists";
        public const string CodeGenerationFailed = "Tenant.CodeGenerationFailed";
    }

    public static class Authorization
    {
        public const string Forbidden = "Authorization.Forbidden";
    }

    public static class MembershipRequest
    {
        public const string NotFound = "MembershipRequest.NotFound";
        public const string InvalidCompanyName = "MembershipRequest.InvalidCompanyName";
        public const string InvalidEmail = "MembershipRequest.InvalidEmail";
        public const string InvalidFirstName = "MembershipRequest.InvalidFirstName";
        public const string InvalidLastName = "MembershipRequest.InvalidLastName";
        public const string InvalidToken = "MembershipRequest.InvalidToken";
        public const string TokenExpired = "MembershipRequest.TokenExpired";
        public const string AlreadyExists = "MembershipRequest.AlreadyExists";
        public const string InvalidStatus = "MembershipRequest.InvalidStatus";
        public const string IdentityProviderError = "MembershipRequest.IdentityProviderError";
        public const string ApprovalSystemError = "MembershipRequest.ApprovalSystemError";
    }

    public static class IdentityAdmin
    {
        public const string UserAlreadyExists = "IdentityAdmin.UserAlreadyExists";
        public const string OperationFailed = "IdentityAdmin.OperationFailed";
    }

    public static class Branch
    {
        public const string NotFound = "Branch.NotFound";
        public const string InvalidTenantId = "Branch.InvalidTenantId";
        public const string InvalidDescription = "Branch.InvalidDescription";
        public const string AlreadyExists = "Branch.AlreadyExists";
    }

    public static class UserRoleAssignment
    {
        public const string InvalidUserId = "UserRoleAssignment.InvalidUserId";
        public const string InvalidRoleId = "UserRoleAssignment.InvalidRoleId";
        public const string InvalidTenantId = "UserRoleAssignment.InvalidTenantId";
        public const string InvalidBranchId = "UserRoleAssignment.InvalidBranchId";
        public const string AlreadyExists = "UserRoleAssignment.AlreadyExists";
        public const string RoleNotAllowedForTenantType = "UserRoleAssignment.RoleNotAllowedForTenantType";
    }
}
