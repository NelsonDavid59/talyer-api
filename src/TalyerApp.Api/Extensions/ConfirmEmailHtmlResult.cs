using System.Net;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Api.Extensions;

internal static class ConfirmEmailHtmlResult
{
    internal static IResult From(
        Result<int> result,
        IEmailTemplateRenderer templates,
        IErrorMessageResolver messages,
        IErrorHttpStatusMapper statuses)
    {
        if (result.IsFailure)
        {
            var html = templates.Render(
                EmailTemplateNames.ConfirmPageError,
                new Dictionary<string, string>
                {
                    [EmailTemplatePlaceholders.Message] =
                        WebUtility.HtmlEncode(messages.ResolveMessage(result.Error))
                });

            return Results.Content(
                html,
                EmailMediaTypes.Html,
                statusCode: statuses.GetStatusCode(result.Error.Category));
        }

        var successHtml = templates.Render(
            EmailTemplateNames.ConfirmPageSuccess,
            new Dictionary<string, string>());

        return Results.Content(successHtml, EmailMediaTypes.Html);
    }
}
