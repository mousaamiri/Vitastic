using Vitastic.Api.Features.Categories.Responses;

namespace Vitastic.Api.Features.Categories.Requests;

public sealed record UpsertCategoryRequest(
    string Name,
    string Slug,
    Guid? ParentCategoryId,
    int? DisplayOrder,
    string? Description = null);
public sealed record UpdateCategoriesListRequest(List<CategoryUpdateResponse> Categories);
public sealed record UpdateCategoryNameRequest(string Name);
public sealed record UpdateCategorySlugRequest(string Slug);
public sealed record UpdateCategoryDisplayOrderRequest(int DisplayOrder);
public sealed record UpdateCategoryDescriptionRequest(string Description);
