namespace Vitastic.Web.Infrastructure.ApiClient;

public interface IApiClient
{
    // ───── GET ─────
    Task<ApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken ct = default);
    Task<ApiResponse<T>> GetAsync<T>(string endpoint, object? queryParams, CancellationToken ct = default);
    Task<PaginatedApiResponse<T>> GetPaginatedAsync<T>(string endpoint, object? queryParams = null, CancellationToken ct = default);

    // ───── POST ─────
    Task<ApiResponse> PostAsync(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? body = null, CancellationToken ct = default);

    // ───── PUT ─────
    Task<ApiResponse> PutAsync(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object? body = null, CancellationToken ct = default);

    // ───── PATCH ─────
    Task<ApiResponse> PatchAsync(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse<T>> PatchAsync<T>(string endpoint, object? body = null, CancellationToken ct = default);

    // ───── DELETE ─────
    Task<ApiResponse> DeleteAsync(string endpoint, CancellationToken ct = default);
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, CancellationToken ct = default);

    // ───── MULTIPART (File Upload) ─────
    Task<ApiResponse> PostMultipartAsync(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse> PutMultipartAsync(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse<T>> PutMultipartAsync<T>(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);

    Task<ApiResponse> PatchMultipartAsync(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse<T>> PatchMultipartAsync<T>(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);

    // ───── DOWNLOAD ─────
    Task<byte[]?> DownloadAsync(string endpoint, CancellationToken ct = default);

    // ───── HELPERS ─────
    MultipartFormDataContent BuildMultipartContent(Dictionary<string, string>? formFields = null, Dictionary<string, (Stream Stream, string FileName)>? files = null);
}
