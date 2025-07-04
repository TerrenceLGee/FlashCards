namespace Flashcards.Core.Results;

public class Result<T> : Result
{
    public T? Value { get; }

    protected internal Result(T? value)
        : base(isSuccess: true, errorMessage: null)
    {
        Value = value;
    }

    protected internal Result(string errorMessage)
        : base(isSuccess: false, errorMessage: errorMessage)
    {
        Value = default;
    }

    public static Result<T> Ok(T value) => new Result<T>(value);
    public static Result<T> Fail(string message) => new Result<T>(message);
}