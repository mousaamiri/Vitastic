namespace Vitastic.App.Features.Categories.Dtos;

public sealed record SubCategoryDto(
    Guid Id,
    string Name,
    string Slug,
    int DisplayOrder);
