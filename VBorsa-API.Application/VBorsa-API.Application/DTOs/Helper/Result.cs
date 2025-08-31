namespace VBorsa_API.Application.DTOs.Helper;

public class Result(bool succeeded, string message, string[] errors)
{
    public bool Succeeded { get; } = succeeded;
    public string Message { get; } = message;
    public string[] Errors { get; } = errors;

    public static Result Success(string message = "")
    {
        return new Result(true, message, []);
    }

    public static Result Failure(params string[] errors)
    {
        return new Result(false, string.Empty, errors);
    }
}

public class Result<T>(bool succeeded, T data, string message, string[] errors)
    : Result(succeeded, message, errors)
{
    public T Data { get; } = data;

    public static Result<T> Success(T data, string message = "")
    {
        return new Result<T>(true, data, message, []);
    }

    public new static Result<T> Failure(params string[] errors)
    {
        return new Result<T>(false, default!, string.Empty, errors);
    }
}