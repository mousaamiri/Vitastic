namespace Vitastic.App.Features.Categories.Dtos;

public sealed record CategoryDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }=string.Empty;
    public string Slug { get; set; }=string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<CategoryDetailDto> SubCategories { get; set; } = [];
}
