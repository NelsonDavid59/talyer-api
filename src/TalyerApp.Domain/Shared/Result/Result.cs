namespace TalyerApp.Domain.Shared.Result;

// Represents the result of an operation, which can either be a success or a failure.
// For operations that do not return data.
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("A successful result cannot have an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("A failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new Result(true, Error.None);

    public static Result Failure(Error error) => new Result(false, error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess ? _value! 
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    private Result(T value) : base(true, Error.None)
    {
        _value = value;
    }

    private Result(Error error) : base(false, error)
    {
        _value = default;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}