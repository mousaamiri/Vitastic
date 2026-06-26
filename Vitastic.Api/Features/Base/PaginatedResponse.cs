namespace Vitastic.Api.Features.Base;

/// <summary>
/// Generic Paginated Response
/// </summary>
public sealed record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);
