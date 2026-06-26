namespace Vitastic.App.Features.Categories.Dtos;

public sealed record CategoryTreeDto(
    Guid Id,
    string Name,
    string Slug,
    Guid? ParentId,
    int DisplayOrder,
    int Level);
