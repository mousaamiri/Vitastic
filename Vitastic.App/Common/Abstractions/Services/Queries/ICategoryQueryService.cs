using Vitastic.App.Features.Categories.Dtos;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.Domain.Entities.Categories.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;
/// <summary>
/// Query Service Interface
/// For READ operations only
///
/// No write logic!
/// Returns DTO directly
/// </summary>
public interface ICategoryQueryService
{
// ==================== LIST ====================
    Task<List<CategoryDetailDto>> GetListedAsync(
        bool? onlyParents = null,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyCollection<CategoryListDto> items,int count)> GetPagedAsync(
        int pageNumber, int pageSize,
        bool? onlyParents = null,
        CancellationToken cancellationToken = default);
    // ==================== DETAIL ====================
    Task<CategoryDetailDto?> GetDetailAsync(
        CategoryId categoryId,
        CancellationToken cancellationToken = default);

    // ==================== SEARCH ====================
    Task<(IReadOnlyList<CategoryListDto> Items, int Total)> SearchAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        bool onlyParents = false,
        bool onlyActive = true,
        CancellationToken cancellationToken = default);

    // ==================== HIERARCHY ====================
    Task<IReadOnlyList<CategoryTreeDto>> GetTreeAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SubCategoryDto>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default);

    // ==================== STATISTICS ====================
    Task<CategoryStatisticsDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default);
}
