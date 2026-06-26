using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Vitastic.Web.Infrastructure.ApiClient;

public class ApiClient(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor,
    ILogger<ApiClient> logger)
    : IApiClient
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    #region ══════════ Private Helpers ══════════

    /// <summary>
    /// Reads the JWT token from Cookie or Session and attaches it to the Authorization header
    /// </summary>
    private void AttachBearerToken()
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null) return;

        // First try to read from Cookie
        var token = httpContext.Request.Cookies["AccessToken"];

        // If not found, try Session
        if (string.IsNullOrEmpty(token))
            token = httpContext.Session.GetString("AccessToken");

        // If still not found, try Claims
        if (string.IsNullOrEmpty(token))
        {
            token = httpContext.User?.Claims
                .FirstOrDefault(c => string.Equals(c.Type, "AccessToken",
                    StringComparison.Ordinal))?.Value;
        }

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        }
    }
    /// <summary>
    /// Attaches the cart session ID to outgoing HTTP requests by reading it from session storage or cookies
    /// and adding it as a custom header (X-Cart-Session-Id) for downstream services.
    /// </summary>
    private void AttachSessionId()
    {
        HttpContext? ctx = httpContextAccessor.HttpContext;
        if (ctx is null) return;

        // Read from session storage or fallback to cookie
        var sessionId = ctx.Session.GetString("cart_session_id")
                        ?? ctx.Request.Cookies["cart_session_id"];

        if (!string.IsNullOrEmpty(sessionId))
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Cart-Session-Id", sessionId);
    }

    /// <summary>
    /// Builds a query string from an object
    /// </summary>
    private string BuildQueryString(string endpoint, object? queryParams)
    {
        if (queryParams is null) return endpoint;

        PropertyInfo[] properties = queryParams.GetType().GetProperties();
        NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

        foreach (PropertyInfo prop in properties)
        {
            var value = prop.GetValue(queryParams);
            if (value is not null)
            {
                queryString[prop.Name] = value.ToString();
            }
        }

        var qs = queryString.ToString();
        return string.IsNullOrEmpty(qs) ? endpoint : $"{endpoint}?{qs}";
    }

    /// <summary>
    /// Serializes an object to JSON StringContent
    /// </summary>
    private StringContent? SerializeBody(object? body)
    {
        if (body is null) return null;

        var json = JsonSerializer.Serialize(body, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Reads and deserializes the API response with typed data
    /// </summary>
    private async Task<ApiResponse<T>> ReadResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken ct)
    {

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            logger.LogWarning("Unauthorized request - User needs to login");

            return ApiResponse<T>.Fail(
                "لطفاً ابتدا وارد حساب کاربری خود شوید",
                401,
                [" احراز هویت نشده - شما نیاز به ورود دارید"]);
        }
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            logger.LogWarning("Forbidden request - User lacks permission");

            return ApiResponse<T>.Fail(
                "شما دسترسی لازم برای این عملیات را ندارید",
                403,
                ["ممنوع - سطح دسترسی شما کافی نیست"]);
        }
        if (response.Content.Headers.ContentLength == 0)
        {
            logger.LogDebug("Response has no content (Content-Length: 0)");

            return response.IsSuccessStatusCode
                ? new ApiResponse<T>
                {
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                    Message = "عملیات موفقیت‌آمیز بود",
                    Data = default
                }
                : ApiResponse<T>.Fail(
                    GetErrorMessageForStatusCode(response.StatusCode),
                    (int)response.StatusCode);
        }
        var content = await response.Content.ReadAsStringAsync(ct);


        logger.LogInformation(
            "API Response [{StatusCode}]: {Content}",
            (int)response.StatusCode,
            content.Length > 500 ? content[..500] + "..." : content);

        //// Handle empty response body
        //if (string.IsNullOrWhiteSpace(content))
        //{
        //    return response.IsSuccessStatusCode
        //        ? new ApiResponse<T>
        //        {
        //            IsSuccess = true,
        //            StatusCode = (int)response.StatusCode,
        //            Message = "عملیات موفقیت‌آمیز بود"
        //        }
        //        : ApiResponse<T>.Fail(
        //            "پاسخی از سرور دریافت نشد",
        //            (int)response.StatusCode);
        //}

        try
        {
            // First attempt: try to deserialize directly into ApiResponse<T>
            ApiResponse<T>? apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);

            if (apiResponse is not null)
            {
                //apiResponse.StatusCode = (int)response.StatusCode;
                //apiResponse.IsSuccess = response.IsSuccessStatusCode;
                return apiResponse;
            }
        }
        catch(Exception ex)
        {
            // If standard envelope format fails, fall through and try deserializing T directly
        }

        try
        {
            T? data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            return new ApiResponse<T>
            {
                IsSuccess = response.IsSuccessStatusCode,
                Data = data,
                StatusCode = (int)response.StatusCode,
                Message = response.IsSuccessStatusCode ? "موفق" : "خطا"
            };
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON Deserialization failed for: {Content}", content);

            return ApiResponse<T>.Fail(
                "خطا در پردازش پاسخ سرور",
                (int)response.StatusCode,
                [ex.Message]);
        }
    }
    /// <summary>
    /// Get user-friendly error message based on HTTP status code
    /// </summary>
    private static string GetErrorMessageForStatusCode(System.Net.HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            System.Net.HttpStatusCode.BadRequest => "درخواست نامعتبر است",
            System.Net.HttpStatusCode.Unauthorized => "لطفاً ابتدا وارد حساب کاربری خود شوید",
            System.Net.HttpStatusCode.Forbidden => "شما دسترسی لازم برای این عملیات را ندارید",
            System.Net.HttpStatusCode.NotFound => "اطلاعات مورد نظر یافت نشد",
            System.Net.HttpStatusCode.Conflict => "تداخل در اطلاعات وجود دارد",
            System.Net.HttpStatusCode.InternalServerError => "خطای سرور رخ داده است",
            System.Net.HttpStatusCode.ServiceUnavailable => "سرویس در دسترس نیست",
            System.Net.HttpStatusCode.RequestTimeout => "زمان درخواست به پایان رسید",
            _ => "خطایی در ارتباط با سرور رخ داده است"
        };
    }
    /// <summary>
    /// Reads and deserializes the API response without typed data
    /// </summary>
    private async Task<ApiResponse> ReadResponseAsync(
        HttpResponseMessage response,
        CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);

        logger.LogInformation(
            "API Response [{StatusCode}]: {Content}",
            (int)response.StatusCode,
            content.Length > 500 ? content[..500] + "..." : content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return response.IsSuccessStatusCode
                ? ApiResponse.Success(statusCode: (int)response.StatusCode)
                : ApiResponse.Fail("پاسخی از سرور دریافت نشد", (int)response.StatusCode);
        }

        try
        {
            ApiResponse? apiResponse = JsonSerializer.Deserialize<ApiResponse>(content, _jsonOptions);
            if (apiResponse is not null)
            {
                apiResponse.StatusCode = (int)response.StatusCode;
                apiResponse.IsSuccess = response.IsSuccessStatusCode;
                return apiResponse;
            }
        }
        catch { }

        return response.IsSuccessStatusCode
            ? ApiResponse.Success(statusCode: (int)response.StatusCode)
            : ApiResponse.Fail(content, (int)response.StatusCode);
    }

    /// <summary>
    /// Handles common exceptions and maps them to ApiResponse
    /// </summary>
    private ApiResponse<T> HandleException<T>(Exception ex, string method, string endpoint)
    {
        logger.LogError(ex, "Error in {Method} {Endpoint}", method, endpoint);

        return ex switch
        {
            HttpRequestException httpEx => ApiResponse<T>.Fail(
                "خطا در ارتباط با سرور",
                statusCode: (int)(httpEx.StatusCode ?? HttpStatusCode.ServiceUnavailable),
                errors: [httpEx.Message]),

            TaskCanceledException => ApiResponse<T>.Fail(
                "درخواست به سرور منقضی شد (Timeout)",
                statusCode: 408),

            JsonException jsonEx => ApiResponse<T>.Fail(
                "خطا در پردازش اطلاعات",
                errors: [jsonEx.Message]),

            _ => ApiResponse<T>.Fail(
                "خطای غیرمنتظره رخ داد",
                statusCode: 500,
                errors: [ex.Message])
        };
    }

    private ApiResponse HandleException(Exception ex, string method, string endpoint)
    {
        logger.LogError(ex, "Error in {Method} {Endpoint}", method, endpoint);

        return ex switch
        {
            HttpRequestException httpEx => ApiResponse.Fail(
                "خطا در ارتباط با سرور",
                statusCode: (int)(httpEx.StatusCode ?? HttpStatusCode.ServiceUnavailable)),

            TaskCanceledException => ApiResponse.Fail(
                "درخواست به سرور منقضی شد",
                statusCode: 408),

            _ => ApiResponse.Fail(
                "خطای غیرمنتظره رخ داد",
                statusCode: 500)
        };
    }

    #endregion

    #region ══════════ GET ══════════

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        return await GetAsync<T>(endpoint, null, ct);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, object? queryParams, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            var url = BuildQueryString(endpoint, queryParams);

            logger.LogInformation("GET {Url}", url);

            HttpResponseMessage response = await httpClient.GetAsync(url, ct);
            return await ReadResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "GET", endpoint);
        }
    }

    public async Task<PaginatedApiResponse<T>> GetPaginatedAsync<T>(
        string endpoint,
        object? queryParams = null,
        CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            var url = BuildQueryString(endpoint, queryParams);

            logger.LogInformation("GET (Paginated) {Url}", url);

            HttpResponseMessage response = await httpClient.GetAsync(url, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(content))
            {
                return new PaginatedApiResponse<T>
                {
                    IsSuccess = false,
                    Message = "پاسخی دریافت نشد",
                    StatusCode = (int)response.StatusCode
                };
            }

            PaginatedApiResponse<T>? result = JsonSerializer.Deserialize<PaginatedApiResponse<T>>(content, _jsonOptions);


            return result ?? new PaginatedApiResponse<T>
            {
                IsSuccess = false,
                Message = "خطا در خواندن اطلاعات",
                StatusCode = (int)response.StatusCode
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GET Paginated {Endpoint}", endpoint);
            return new PaginatedApiResponse<T>
            {
                IsSuccess = false,
                Message = ex.Message,
                StatusCode = 500
            };
        }
    }

    #endregion

    #region ══════════ POST ══════════

    public async Task<ApiResponse> PostAsync(string endpoint, object? body = null, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("POST {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.PostAsync(endpoint, SerializeBody(body), ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "POST", endpoint);
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? body = null, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();
            AttachSessionId();
            logger.LogInformation("POST {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.PostAsync(endpoint, SerializeBody(body), ct);
            return await ReadResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "POST", endpoint);
        }
    }

    #endregion

    #region ══════════ PUT ══════════

    public async Task<ApiResponse> PutAsync(string endpoint, object? body = null, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("PUT {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.PutAsync(endpoint, SerializeBody(body), ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "PUT", endpoint);
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object? body = null, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("PUT {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.PutAsync(endpoint, SerializeBody(body), ct);
            return await ReadResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "PUT", endpoint);
        }
    }

    #endregion

    #region ══════════ PATCH ══════════

    public async Task<ApiResponse> PatchAsync(string endpoint, object? body = null, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("PATCH {Endpoint}", endpoint);

            StringContent? patchContent = SerializeBody(body);
            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = patchContent
            };

            HttpResponseMessage response = await httpClient.SendAsync(request, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "PATCH", endpoint);
        }
    }

    public async Task<ApiResponse<T>> PatchAsync<T>(string endpoint, object? body = null, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("PATCH {Endpoint}", endpoint);

            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = SerializeBody(body)
            };

            HttpResponseMessage response = await httpClient.SendAsync(request, ct);
            return await ReadResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "PATCH", endpoint);
        }
    }

    #endregion

    #region ══════════ DELETE ══════════

    public async Task<ApiResponse> DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("DELETE {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.DeleteAsync(endpoint, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DELETE", endpoint);
        }
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("DELETE {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.DeleteAsync(endpoint, ct);
            return await ReadResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "DELETE", endpoint);
        }
    }

    #endregion

    #region ══════════ MULTIPART (File Upload) ══════════

    public async Task<ApiResponse> PostMultipartAsync(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("POST Multipart {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "POST-Multipart", endpoint);
        }
    }

    public async Task<ApiResponse<T>> PostMultipartAsync<T>(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("POST Multipart {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content, ct);
            return await ReadResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "POST-Multipart", endpoint);
        }
    }

    public async Task<ApiResponse<T>> PutMultipartAsync<T>(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("PUT Multipart {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.PutAsync(endpoint, content, ct);
            return await ReadResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "PUT-Multipart", endpoint);
        }
    }
    public async Task<ApiResponse> PutMultipartAsync(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken(); AttachSessionId();
            logger.LogInformation("PUT Multipart {Endpoint}", endpoint);

            HttpResponseMessage response = await httpClient.PutAsync(endpoint, content, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "PUT-Multipart", endpoint);
        }
    }
    public async Task<ApiResponse> PatchMultipartAsync(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();
            AttachSessionId();
            logger.LogInformation("PATCH Multipart {Endpoint}", endpoint);

            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = content
            };

            HttpResponseMessage response = await httpClient.SendAsync(request, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "PATCH-Multipart", endpoint);
        }
    }

    public async Task<ApiResponse<T>> PatchMultipartAsync<T>(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();
            AttachSessionId();
            logger.LogInformation("PATCH Multipart {Endpoint}", endpoint);

            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = content
            };

            HttpResponseMessage response = await httpClient.SendAsync(request, ct);
            return await ReadResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, "PATCH-Multipart", endpoint);
        }
    }

    #endregion


    #region ══════════ DOWNLOAD ══════════

    public async Task<byte[]?> DownloadAsync(string endpoint, CancellationToken ct = default)
    {
        try
        {
            AttachBearerToken();AttachSessionId();
            logger.LogInformation("DOWNLOAD {Endpoint}", endpoint);
            HttpResponseMessage response = await httpClient.GetAsync(endpoint, ct);


            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsByteArrayAsync(ct);

            logger.LogWarning("Download failed with status {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading from {Endpoint}", endpoint);
            return null;
        }
    }

    #endregion

    #region ══════════ HELPERS ══════════

    public MultipartFormDataContent BuildMultipartContent(
        Dictionary<string, string>? formFields = null,
        Dictionary<string, (Stream Stream, string FileName)>? files = null)
    {
        var content = new MultipartFormDataContent();

        // Add form fields
        if (formFields is not null)
        {
            foreach (KeyValuePair<string, string> field in formFields)
            {
                content.Add(new StringContent(field.Value, Encoding.UTF8), field.Key);
            }
        }

        // Add file streams
        if (files is not null)
        {
            foreach (KeyValuePair<string, (Stream Stream, string FileName)> file in files)
            {
                var streamContent = new StreamContent(file.Value.Stream);
                streamContent.Headers.ContentType =
                    new MediaTypeHeaderValue(GetMimeType(file.Value.FileName));

                content.Add(streamContent, file.Key, file.Value.FileName);
            }
        }

        return content;
    }

    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".pdf" => "application/pdf",
            ".mp4" => "video/mp4",
            ".mp3" => "audio/mpeg",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }

    #endregion
}

internal static class FormFileExtensions
{
    /// <summary>
    /// Convert IFormFile to MultipartFormDataContent
    /// </summary>
    public static MultipartFormDataContent ToMultipartContent(
        this IFormFile file,
        string fieldName = "file",
        Dictionary<string, string>? additionalFields = null)
    {
        var content = new MultipartFormDataContent();

        Stream stream = file.OpenReadStream();
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

        content.Add(streamContent, fieldName, file.FileName);

        if (additionalFields is not null)
        {
            foreach (KeyValuePair<string, string> field in additionalFields)
            {
                content.Add(new StringContent(field.Value, Encoding.UTF8), field.Key);
            }
        }

        return content;
    }

    /// <summary>
    /// Multi convert IFormFile to MultipartFormDataContent
    /// </summary>
    public static MultipartFormDataContent ToMultipartContent(
        this IEnumerable<IFormFile> files,
        string fieldName = "files",
        Dictionary<string, string>? additionalFields = null)
    {
        var content = new MultipartFormDataContent();

        foreach (IFormFile file in files)
        {
            Stream stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

            content.Add(streamContent, fieldName, file.FileName);
        }

        if (additionalFields is not null)
        {
            foreach (KeyValuePair<string, string> field in additionalFields)
            {
                content.Add(new StringContent(field.Value, Encoding.UTF8), field.Key);
            }
        }

        return content;
    }

    public static void AddObject<T>(
        this MultipartFormDataContent content,
        T obj,
        string? prefix = null) where T : class
    {
        if (obj is null) return;

        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in properties)
        {
            object? value = prop.GetValue(obj);
            if (value is null) continue;

            string fieldName = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";

            // Handle IFormFile
            if (value is IFormFile file)
            {
                var streamContent = new StreamContent(file.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                    file.ContentType ?? "application/octet-stream");
                content.Add(streamContent, fieldName, file.FileName);
            }
            // Handle collections (but not string)
            else if (value is IEnumerable enumerable && value is not string)
            {
                int index = 0;
                foreach (object? item in enumerable)
                {
                    if (item is null) continue;

                    // If item is complex type, recurse
                    if (item.GetType().IsClass && item is not string && item is not IFormFile)
                    {
                        content.AddObject(item, $"{fieldName}[{index}]");
                    }
                    else
                    {
                        content.Add(
                            new StringContent(item.ToString() ?? string.Empty, Encoding.UTF8),
                            $"{fieldName}[{index}]");
                    }
                    index++;
                }
            }
            // Handle simple types
            else if (prop.PropertyType.IsPrimitive ||
                     prop.PropertyType == typeof(string) ||
                     prop.PropertyType == typeof(Guid) ||
                     prop.PropertyType == typeof(decimal) ||
                     prop.PropertyType == typeof(DateTime) ||
                     prop.PropertyType == typeof(TimeSpan))
            {
                content.Add(
                    new StringContent(value.ToString() ?? string.Empty, Encoding.UTF8),
                    fieldName);
            }
            // Handle complex nested objects
            else if (prop.PropertyType.IsClass)
            {
                content.AddObject(value, fieldName);
            }
        }
    }

}

