namespace Carma.Application.Common;

public record Result {
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
  
    public Result(bool isSuccess, string? error) {
        IsSuccess = isSuccess;
        Error = error;
    }
  
    public static Result Success() => new (true, null);
    public static Result Failure(string error) => new (false, error);
}

public record Result<T>
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public T? Value { get; init; }
    
    public Result(bool isSuccess, string? error, T? value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public static Result<T> Success(T value) => new (true, null, value);
    public static Result<T> Failure(string error) => new (false, error, default);
}