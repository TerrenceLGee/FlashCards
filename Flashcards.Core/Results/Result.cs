namespace Flashcards.Core.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage { get; }

    protected Result(bool isSuccess, string? errorMessage)
    {
        if (isSuccess && errorMessage != null)
            throw new InvalidOperationException("Success cannot have an error message");
        if (!isSuccess && errorMessage == null)
            throw new InvalidOperationException("Failure must have an error message");

        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Ok() => new Result(true, null);
    public static Result Fail(string message) => new Result(false, message);
}