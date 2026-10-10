namespace TalyerApp.Domain.Shared.Result;

public sealed record Error(string Code, ErrorCategory Category) : IDomainError
{
    public static readonly Error None = new(string.Empty, ErrorCategory.Validation);

    public static Error NotFound(string code) => new(code, ErrorCategory.NotFound);

    public static Error Validation(string code) => new(code, ErrorCategory.Validation);

    public static Error Conflict(string code) => new(code, ErrorCategory.Conflict);

    public static Error Forbidden(string code) => new(code, ErrorCategory.Forbidden);

    public static Error System(string code) => new(code, ErrorCategory.System);
}
