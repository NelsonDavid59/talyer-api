public sealed record Error(string Code, string Message)
{
    // Represents the absence of an error.
    public static readonly Error None = new Error(string.Empty, string.Empty);

    public static Error NotFound(string code, string message) => new Error(code, message);

    public static Error Validation(string code, string message) => new Error(code, message);
}