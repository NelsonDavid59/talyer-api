using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Persistence;

public interface IUniqueConstraintRegistry
{
    Error? TryGetUniqueViolation(string? constraintName, string? tableName);
}
