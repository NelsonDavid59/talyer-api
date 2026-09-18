namespace TalyerApp.Domain.Shared;

public static class DomainErrors
{
    public static class UserProfile
    {
        public static readonly Error UserProfileNotFound = 
            Error.NotFound("UserProfile.NotFound", "User profile not found.");
        
        public static readonly Error InvalidUserProfileFirstNameFormat = 
            Error.Validation("UserProfile.InvalidFirstName", "User profile first name format is invalid.");

        public static readonly Error InvalidUserProfileLastNameFormat = 
            Error.Validation("UserProfile.InvalidLastName", "User profile last name format is invalid.");
    }

    public static class RefreshToken
    {
        public static readonly Error RefreshTokenNotFound = 
            Error.NotFound("RefreshToken.NotFound", "Refresh token not found.");
        
        public static readonly Error InvalidTokenFormat = 
            Error.Validation("RefreshToken.InvalidTokenFormat", "Refresh token format is invalid.");

        public static readonly Error InvalidExpirationDate = 
            Error.Validation("RefreshToken.InvalidExpirationDate", "Refresh token expiration date is invalid.");
    }
}