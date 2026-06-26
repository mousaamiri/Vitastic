using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Base;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse Success(string message = "عملیات موفقیت‌آمیز بود", int statusCode = 200)
    {
        return new ApiResponse
        {
            IsSuccess = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ApiResponse Fail(string message,  ErrorType errorType=ErrorType.Validation, List<string>? errors = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Message = message,
            StatusCode = MapErrorTypeToStatusCode(errorType),
            Errors = errors ?? []
        };
    }
    protected static int MapErrorTypeToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Verification => StatusCodes.Status422UnprocessableEntity,
        ErrorType.Failure => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
    };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "عملیات موفقیت‌آمیز بود", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }

    public new static ApiResponse<T> Fail(string message,  ErrorType errorType=ErrorType.Validation, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Data = default,
            Message = message,
            StatusCode = MapErrorTypeToStatusCode(errorType),
            Errors = errors ?? []
        };
    }
}
