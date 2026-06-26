namespace Vitastic.Web.Models.DTOs.Category;

public sealed record CategoryDetailDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    int DisplayOrder,
    bool IsActive,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    IReadOnlyList<CategoryDetailDto> SubCategories);
public sealed record SubCategoryDto(
    Guid Id,
    string Name,
    string Slug,
    int DisplayOrder);

public sealed record CategoryListDto(
    Guid Id,
    string Name,
    string Slug,
    int DisplayOrder,
    bool IsActive,
    Guid? ParentId);

public sealed record UpsertCategoryRequest(
    string Name,
    string Slug,
    Guid? ParentCategoryId,
    int? DisplayOrder,
    string? Description = null);
public class CategorySelectViewModel
{
    public IReadOnlyCollection<CategoryDetailDto> Categories { get; set; } = [];    
    public int Level { get; set; }
}
public class CategoryUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
}
