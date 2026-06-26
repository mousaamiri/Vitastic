namespace Vitastic.App.Features.Categories.Dtos;

public sealed record CategoryStatisticsDto(
    int TotalCategories,
    int ActiveCategories,
    int SubCategoriesCount,
    int ParentCategoriesCount);
