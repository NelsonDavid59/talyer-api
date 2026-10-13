namespace TalyerApp.Domain.Authorization;

public static class TalyerClaimTypes
{
    /// <summary>
    /// Internal user id (Users.Id) added after JWT validation and RegisterExternalIdentityCmd.
    /// </summary>
    public const string UserId = "talyer_user_id";
}
