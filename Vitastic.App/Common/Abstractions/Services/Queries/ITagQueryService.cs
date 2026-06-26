using Vitastic.App.Features.Tags.Dtos;
using Vitastic.Domain.Entities.Tags.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;
/// <summary>
/// Query Service Interface
/// For READ operations only
/// </summary>
public interface ITagQueryService
{
// ==================== LIST ====================
    Task<(IReadOnlyList<TagDto> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool? onlyActive = true,
        CancellationToken cancellationToken = default);

    // ==================== DETAIL ====================
    Task<TagDto?> GetDetailAsync(
        TagId tagId,
        CancellationToken cancellationToken = default);

    // ==================== SEARCH ====================
    Task<(IReadOnlyList<TagDto> Items, int Total)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        bool? onlyActive = true,
        CancellationToken cancellationToken = default);

    // ==================== USAGE ====================
    Task<(IReadOnlyList<TagDto> Items, int Total)> GetMostUsedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TagDto>> GetByUsageRangeAsync(
        int minUsage,
        int maxUsage,
        CancellationToken cancellationToken = default);
    // ==================== STATUS ====================
    Task<IReadOnlyList<TagDto>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TagDto>> GetInactiveAsync(
        CancellationToken cancellationToken = default);

    // ==================== STATISTICS ====================
    Task<TagStatisticsDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default);
}
