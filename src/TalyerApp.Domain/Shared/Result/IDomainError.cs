namespace TalyerApp.Domain.Shared.Result;

public interface IDomainError
{
    string Code { get; }
    ErrorCategory Category { get; }
}
