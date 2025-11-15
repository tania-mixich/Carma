using Carma.Application.Common;

namespace Carma.Application.Common;

public record Result {
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public ErrorType? ErrorType { get; init; }
  
    public Result(bool isSuccess, string? error, ErrorType? errorType) {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }
  
    public static Result Success() => new (true, null, Common.ErrorType.None);
    public static Result Failure(string error) => new (false, error, Common.ErrorType.Validation);
    public static Result NotFound(string error) => new (false, error, Common.ErrorType.NotFound);
    public static Result Unauthorized(string error) => new (false, error, Common.ErrorType.Unauthorized);
    public static Result Conflict(string error) => new (false, error, Common.ErrorType.Conflict);
}

public record Result<T>
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public T? Value { get; init; }
    public ErrorType? ErrorType { get; init; }
    
    public Result(bool isSuccess, string? error, T? value, ErrorType? errorType)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new (true, null, value, Common.ErrorType.None);
    public static Result<T> Failure(string error) => new (false, error, default, Common.ErrorType.Validation);
    public static Result<T> NotFound(string error) => new (false, error, default, Common.ErrorType.NotFound);
    public static Result<T> Unauthorized(string error) => new (false, error, default, Common.ErrorType.Unauthorized);
    public static Result<T> Conflict(string error) => new (false, error, default, Common.ErrorType.Conflict);
}