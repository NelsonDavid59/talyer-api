using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence;

public sealed record UniqueConstraintRegistration(string ConstraintName, Error Error);

public sealed record UniqueConstraintTableFallback(string TableName, Error Error);
