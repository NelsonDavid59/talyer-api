namespace TalyerApp.Domain.Shared.Membership;

public static class MembershipConfirmUrl
{
    public static string Build(string baseUrl, string token)
    {
        var origin = baseUrl.TrimEnd('/');
        return $"{origin}{MembershipRoutes.ConfirmEmail}?{MembershipQueryNames.Token}={Uri.EscapeDataString(token)}";
    }
}
