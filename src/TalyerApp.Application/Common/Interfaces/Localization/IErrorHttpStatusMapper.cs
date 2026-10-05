using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Localization;

public interface IErrorHttpStatusMapper
{
    int GetStatusCode(ErrorCategory category);
}
