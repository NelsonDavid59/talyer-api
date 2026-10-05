using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Persistence;

public interface IDatabaseExceptionTranslator
{
    Error? FromSaveChanges(Exception exception, DatabaseExceptionContext? context = null);
}
