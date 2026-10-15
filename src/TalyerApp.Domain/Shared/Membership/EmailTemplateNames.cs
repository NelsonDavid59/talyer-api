namespace TalyerApp.Domain.Shared.Membership;

public static class EmailTemplateNames
{
    public const string ConfirmRequest = "confirm-request.html";
    public const string EmailConfirmed = "email-confirmed.html";
    public const string ConfirmPageSuccess = "confirm-page-success.html";
    public const string ConfirmPageError = "confirm-page-error.html";

    public static readonly string[] All =
    [
        ConfirmRequest,
        EmailConfirmed,
        ConfirmPageSuccess,
        ConfirmPageError
    ];
}
