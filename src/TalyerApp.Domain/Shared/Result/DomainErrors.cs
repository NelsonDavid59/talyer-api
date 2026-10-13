namespace TalyerApp.Domain.Shared.Result;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error UserNotFound = Error.NotFound(ErrorCodes.User.NotFound);

        public static readonly Error InvalidUserEmailFormat =
            Error.Validation(ErrorCodes.User.InvalidEmail);

        public static readonly Error InvalidUserUsernameFormat =
            Error.Validation(ErrorCodes.User.InvalidUsername);

        public static readonly Error InvalidUserFirstNameFormat =
            Error.Validation(ErrorCodes.User.InvalidFirstName);

        public static readonly Error InvalidUserLastNameFormat =
            Error.Validation(ErrorCodes.User.InvalidLastName);

        public static readonly Error AlreadyExists =
            Error.Conflict(ErrorCodes.User.AlreadyExists);
    }

    public static class ExternalIdentity
    {
        public static readonly Error ExternalIdentityNotFound =
            Error.NotFound(ErrorCodes.ExternalIdentity.NotFound);

        public static readonly Error InvalidExternalIdentityProvider =
            Error.Validation(ErrorCodes.ExternalIdentity.InvalidProvider);

        public static readonly Error InvalidExternalIdentityProviderUserId =
            Error.Validation(ErrorCodes.ExternalIdentity.InvalidProviderUserId);

        public static readonly Error AlreadyExists =
            Error.Conflict(ErrorCodes.ExternalIdentity.AlreadyExists);
    }

    public static class Role
    {
        public static readonly Error RoleNotFound = Error.NotFound(ErrorCodes.Role.NotFound);
        public static readonly Error InvalidRoleName = Error.Validation(ErrorCodes.Role.InvalidRoleName);
        public static readonly Error InvalidRoleCode = Error.Validation(ErrorCodes.Role.InvalidCode);
    }

    public static class Permission
    {
        public static readonly Error PermissionNotFound = Error.NotFound(ErrorCodes.Permission.NotFound);
        public static readonly Error PermissionInvalidCode = Error.Validation(ErrorCodes.Permission.InvalidCode);
    }

    public static class RolePermission
    {
        public static readonly Error RolePermissionNotFound =
            Error.NotFound(ErrorCodes.RolePermission.NotFound);

        public static readonly Error RolePermissionInvalidRoleId =
            Error.Validation(ErrorCodes.RolePermission.InvalidRoleId);

        public static readonly Error RolePermissionInvalidPermissionId =
            Error.Validation(ErrorCodes.RolePermission.InvalidPermissionId);

        public static readonly Error AlreadyExists =
            Error.Conflict(ErrorCodes.RolePermission.AlreadyExists);
    }

    public static class Tenant
    {
        public static readonly Error TenantNotFound = Error.NotFound(ErrorCodes.Tenant.NotFound);
        public static readonly Error TenantInvalidDescription =
            Error.Validation(ErrorCodes.Tenant.InvalidDescription);

        public static readonly Error TenantInvalidCode =
            Error.Validation(ErrorCodes.Tenant.InvalidCode);

        public static readonly Error AlreadyExists =
            Error.Conflict(ErrorCodes.Tenant.AlreadyExists);

        public static readonly Error CodeGenerationFailed =
            Error.Conflict(ErrorCodes.Tenant.CodeGenerationFailed);
    }

    public static class Authorization
    {
        public static readonly Error Forbidden =
            Error.Forbidden(ErrorCodes.Authorization.Forbidden);
    }

    public static class MembershipRequest
    {
        public static readonly Error NotFound =
            Error.NotFound(ErrorCodes.MembershipRequest.NotFound);

        public static readonly Error InvalidCompanyName =
            Error.Validation(ErrorCodes.MembershipRequest.InvalidCompanyName);

        public static readonly Error InvalidEmail =
            Error.Validation(ErrorCodes.MembershipRequest.InvalidEmail);

        public static readonly Error InvalidFirstName =
            Error.Validation(ErrorCodes.MembershipRequest.InvalidFirstName);

        public static readonly Error InvalidLastName =
            Error.Validation(ErrorCodes.MembershipRequest.InvalidLastName);

        public static readonly Error InvalidToken =
            Error.Validation(ErrorCodes.MembershipRequest.InvalidToken);

        public static readonly Error TokenExpired =
            Error.Validation(ErrorCodes.MembershipRequest.TokenExpired);

        public static readonly Error AlreadyExists =
            Error.Conflict(ErrorCodes.MembershipRequest.AlreadyExists);

        public static readonly Error InvalidStatus =
            Error.Validation(ErrorCodes.MembershipRequest.InvalidStatus);

        public static readonly Error IdentityProviderError =
            Error.Validation(ErrorCodes.MembershipRequest.IdentityProviderError);

        public static readonly Error ApprovalSystemError =
            Error.System(ErrorCodes.MembershipRequest.ApprovalSystemError);
    }

    public static class IdentityAdmin
    {
        public static readonly Error UserAlreadyExists =
            Error.Conflict(ErrorCodes.IdentityAdmin.UserAlreadyExists);

        public static readonly Error OperationFailed =
            Error.Validation(ErrorCodes.IdentityAdmin.OperationFailed);
    }

    public static class Branch
    {
        public static readonly Error BranchNotFound = Error.NotFound(ErrorCodes.Branch.NotFound);
        public static readonly Error BranchInvalidTenantId =
            Error.Validation(ErrorCodes.Branch.InvalidTenantId);
        public static readonly Error BranchInvalidDescription =
            Error.Validation(ErrorCodes.Branch.InvalidDescription);

        public static readonly Error AlreadyExists =
            Error.Conflict(ErrorCodes.Branch.AlreadyExists);
    }

    public static class UserRoleAssignment
    {
        public static readonly Error InvalidUserId =
            Error.Validation(ErrorCodes.UserRoleAssignment.InvalidUserId);

        public static readonly Error InvalidRoleId =
            Error.Validation(ErrorCodes.UserRoleAssignment.InvalidRoleId);

        public static readonly Error InvalidTenantId =
            Error.Validation(ErrorCodes.UserRoleAssignment.InvalidTenantId);

        public static readonly Error InvalidBranchId =
            Error.Validation(ErrorCodes.UserRoleAssignment.InvalidBranchId);

        public static readonly Error AlreadyExists =
            Error.Conflict(ErrorCodes.UserRoleAssignment.AlreadyExists);

        public static readonly Error RoleNotAllowedForTenantType =
            Error.Validation(ErrorCodes.UserRoleAssignment.RoleNotAllowedForTenantType);
    }
}
