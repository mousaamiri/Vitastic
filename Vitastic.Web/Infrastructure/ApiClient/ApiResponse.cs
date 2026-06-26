namespace Vitastic.Web.Infrastructure.ApiClient;

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

    public static ApiResponse Fail(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? []
        };
    }
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

    public new static ApiResponse<T> Fail(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Data = default,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? []
        };
    }
}
