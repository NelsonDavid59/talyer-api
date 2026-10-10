using System.Security.Claims;
using TalyerApp.Application.Common.Authorization;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Api.Extensions;

namespace TalyerApp.Api.Endpoints;

internal static class PlatformAdminEndpointAuthorization
{
    internal static async Task<IResult?> EnsurePlatformAdminAsync(
        HttpContext httpContext,
        IPlatformAdminAuthorizer platformAdminAuthorizer)
    {
        var userIdValue = httpContext.User.FindFirstValue("talyer_user_id");
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Results.Unauthorized();
        }

        var authResult = await platformAdminAuthorizer.EnsureIsPlatformAdminAsync(userId, httpContext.RequestAborted);
        if (authResult.IsFailure)
        {
            var messages = httpContext.RequestServices.GetRequiredService<IErrorMessageResolver>();
            var statuses = httpContext.RequestServices.GetRequiredService<IErrorHttpStatusMapper>();
            return authResult.Error.ToProblemResult(messages, statuses);
        }

        return null;
    }
}
