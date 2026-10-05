using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Api.Extensions;

public static class ErrorResponseExtensions
{
    public static IResult ToProblemResult(
        this Error error,
        IErrorMessageResolver messageResolver,
        IErrorHttpStatusMapper statusMapper)
    {
        var statusCode = statusMapper.GetStatusCode(error.Category);
        var message = messageResolver.ResolveMessage(error);

        return Results.Json(
            new
            {
                code = error.Code,
                category = error.Category.ToString(),
                message
            },
            statusCode: statusCode);
    }
}
