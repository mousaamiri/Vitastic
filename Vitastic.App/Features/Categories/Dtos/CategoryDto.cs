namespace Vitastic.App.Features.Categories.Dtos;
public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    int DisplayOrder,
    bool IsActive,
    Guid? ParentCategoryId=null
    );
