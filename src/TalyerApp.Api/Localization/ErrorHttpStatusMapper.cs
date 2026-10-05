using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Api.Localization;

public class ErrorHttpStatusMapper : IErrorHttpStatusMapper
{
    public int GetStatusCode(ErrorCategory category) => category switch
    {
        ErrorCategory.NotFound => StatusCodes.Status404NotFound,
        ErrorCategory.Validation => StatusCodes.Status400BadRequest,
        ErrorCategory.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
