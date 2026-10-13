using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Localization;

public interface IErrorMessageResolver
{
    string ResolveMessage(IDomainError error);
}
