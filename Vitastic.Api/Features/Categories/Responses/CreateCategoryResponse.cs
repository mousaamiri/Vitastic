namespace Vitastic.Api.Features.Categories.Responses;

/// <summary>
/// Category Detail Response
/// </summary>
public sealed record CategoryDetailResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    int DisplayOrder,
    bool IsActive,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    IReadOnlyList<CategoryDetailResponse> SubCategories);
public sealed record SubCategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    int DisplayOrder);

/// <summary>
/// Category List Item Response
/// </summary>
public sealed record CategoryListResponse(
    Guid Id,
    string Name,
    string Slug,
    int DisplayOrder,
    bool IsActive,
    Guid? ParentId);
public class CategoryUpdateResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
}
