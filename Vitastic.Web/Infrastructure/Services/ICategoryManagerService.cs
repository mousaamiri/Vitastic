using Vitastic.Web.Areas.AdminPanel.Controllers;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs;
using Vitastic.Web.Models.DTOs.Category;

namespace Vitastic.Web.Infrastructure.Services;

public interface ICategoryManagerService
{
    Task<ApiResponse<List<CategoryDetailDto>>> GetAllAsync(CancellationToken ct = default);
    Task<PaginatedApiResponse<CategoryViewModel>> GetTopCategoriesAsync(int count, CancellationToken token);
    Task<ApiResponse<CategoryDetailDto>> GetByIdAsync(Guid categoryId, CancellationToken ct = default);
    Task<ApiResponse<Guid>> CreateNewAsync(UpsertCategoryRequest request, CancellationToken ct = default);
    Task<ApiResponse<Guid>> UpdateAsync(Guid categoryId, UpsertCategoryRequest request, CancellationToken ct = default);
    Task<ApiResponse<Guid>> DeleteAsync(Guid categoryId, CancellationToken ct = default);
    Task<ApiResponse> UpdateListAsync(List<CategoryUpdateDto> request, CancellationToken ct = default);

}

public sealed class CategoryMangerService(IApiClient apiClient) : ICategoryManagerService
{
    public async Task<ApiResponse<List<CategoryDetailDto>>> GetAllAsync(CancellationToken ct = default)
        => await apiClient.GetAsync<List<CategoryDetailDto>>("Categories/tree", ct);

    public async Task<PaginatedApiResponse<CategoryViewModel>> GetTopCategoriesAsync(int count, CancellationToken token)
    => await apiClient.GetPaginatedAsync<CategoryViewModel>(
                "Categories", new { pageNumber = 1, pageSize = count, onlyParents = false }, token);


    public async Task<ApiResponse<CategoryDetailDto>> GetByIdAsync(Guid categoryId, CancellationToken ct = default)
        => await apiClient.GetAsync<CategoryDetailDto>($"Categories/{categoryId}", ct);

    public async Task<ApiResponse<Guid>> CreateNewAsync(UpsertCategoryRequest request, CancellationToken ct = default)
        => await apiClient.PostAsync<Guid>("Categories", request, ct);

    public async Task<ApiResponse<Guid>> UpdateAsync(Guid categoryId, UpsertCategoryRequest request, CancellationToken ct = default)
        => await apiClient.PutAsync<Guid>($"Categories/{categoryId}", request, ct);

    public async Task<ApiResponse<Guid>> DeleteAsync(Guid categoryId, CancellationToken ct = default)
        => await apiClient.DeleteAsync<Guid>($"Categories/{categoryId}", ct);

    public async Task<ApiResponse> UpdateListAsync(List<CategoryUpdateDto> request, CancellationToken ct = default)
        => await apiClient.PutAsync("Categories/update-range", new { categories = request }, ct);

}
