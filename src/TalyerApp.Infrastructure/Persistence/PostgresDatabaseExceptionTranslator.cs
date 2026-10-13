using Npgsql;
using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Persistence;

public class PostgresDatabaseExceptionTranslator : IDatabaseExceptionTranslator
{
    private readonly IUniqueConstraintRegistry _uniqueConstraintRegistry;

    public PostgresDatabaseExceptionTranslator(IUniqueConstraintRegistry uniqueConstraintRegistry)
    {
        _uniqueConstraintRegistry = uniqueConstraintRegistry;
    }

    public Error? FromSaveChanges(Exception exception, DatabaseExceptionContext? context = null)
    {
        var postgresException = FindPostgresException(exception);
        if (postgresException is null)
        {
            return null;
        }

        if (postgresException.SqlState != PostgresErrorCodes.UniqueViolation)
        {
            return null;
        }

        if (context?.UniqueViolationError is not null)
        {
            return context.UniqueViolationError;
        }

        return _uniqueConstraintRegistry.TryGetUniqueViolation(
            postgresException.ConstraintName,
            postgresException.TableName);
    }

    private static PostgresException? FindPostgresException(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgresException)
            {
                return postgresException;
            }
        }

        return null;
    }
}
