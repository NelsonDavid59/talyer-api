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

        public static readonly Error InvalidUserNameFormat = 
            Error.Validation("User.InvalidName", "User name format is invalid.");

        public static readonly Error InvalidUserLastNameFormat = 
            Error.Validation("User.InvalidLastName", "User last name format is invalid.");
    }

    public static class ExternalIdentity
    {
        public static readonly Error InvalidExternalIdentityProvider = 
            Error.Validation("ExternalIdentity.InvalidProvider", "External identity provider is invalid.");

        public static readonly Error InvalidExternalIdentityProviderUserId = 
            Error.Validation("ExternalIdentity.InvalidProviderUserId", "External identity provider user ID is invalid.");
    }
}