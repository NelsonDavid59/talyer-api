using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Persistence;

public sealed record DatabaseExceptionContext(Error? UniqueViolationError = null);
