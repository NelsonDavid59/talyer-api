using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence;

public class UniqueConstraintRegistry : IUniqueConstraintRegistry
{
    private readonly Dictionary<string, Error> _byConstraintName;
    private readonly Dictionary<string, Error> _byTableName;

    public UniqueConstraintRegistry(
        IEnumerable<UniqueConstraintRegistration> constraints,
        IEnumerable<UniqueConstraintTableFallback> tableFallbacks)
    {
        _byConstraintName = constraints.ToDictionary(
            x => x.ConstraintName,
            x => x.Error,
            StringComparer.Ordinal);

        _byTableName = tableFallbacks.ToDictionary(
            x => x.TableName,
            x => x.Error,
            StringComparer.Ordinal);
    }

    public static UniqueConstraintRegistry Create() =>
        new(
            UniqueConstraintMappings.GetConstraintRegistrations(),
            UniqueConstraintMappings.GetTableFallbacks());

    public Error? TryGetUniqueViolation(string? constraintName, string? tableName)
    {
        if (!string.IsNullOrEmpty(constraintName)
            && _byConstraintName.TryGetValue(constraintName, out var errorByConstraint))
        {
            return errorByConstraint;
        }

        if (!string.IsNullOrEmpty(tableName)
            && _byTableName.TryGetValue(tableName, out var errorByTable))
        {
            return errorByTable;
        }

        return null;
    }
}
