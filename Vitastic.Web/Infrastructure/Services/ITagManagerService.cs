using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Tag;

namespace Vitastic.Web.Infrastructure.Services;

public interface ITagManagerService
{
    Task<ApiResponse<PaginatedData<TagDto>>> GetAllAsync(CancellationToken ct = default);
    Task<ApiResponse<Guid>> CreateTagAsync(string tagName, CancellationToken ct = default);
    Task<ApiResponse> updateTagAsyncTask(Guid tagId, string tagName, CancellationToken ct = default);
    Task<ApiResponse<PaginatedData<TagDto>>> SearchAsync(string? searchQuery, int pageNumber, int take, CancellationToken ct = default);
    Task<ApiResponse> DeleteTagAsync(Guid id,CancellationToken ct=default);
}

public sealed class TagManagerService(IApiClient apiClient) : ITagManagerService
{
    public async Task<ApiResponse<PaginatedData<TagDto>>> GetAllAsync(CancellationToken ct = default)
        => await apiClient.GetPaginatedAsync<TagDto>("Tags", ct: ct);

    public async Task<ApiResponse<Guid>> CreateTagAsync(string tagName, CancellationToken ct = default)
        => await apiClient.PostAsync<Guid>("Tags", new { name = tagName }, ct: ct);


    public async Task<ApiResponse> updateTagAsyncTask(Guid tagId, string tagName, CancellationToken ct = default)
        => await apiClient.PatchAsync($"Tags/{tagId}/name", new { newName = tagName }, ct: ct);

    public async Task<ApiResponse<PaginatedData<TagDto>>>
        SearchAsync(string? searchQuery, int pageNumber, int take, CancellationToken ct = default)
        => await apiClient.GetPaginatedAsync<TagDto>("Tags/search",new
        {
            searchTerm =searchQuery,
            pageNumber=pageNumber,
            pageSize=take
        }, ct: ct);

    public async Task<ApiResponse> DeleteTagAsync(Guid id, CancellationToken ct = default)
        => await apiClient.DeleteAsync($"Tags/{id}", ct: ct);
}
