using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Categories.Dtos;

namespace Vitastic.App.Features.Categories.Queries.GetCategoriesTree;

public record GetCategoriesTreeQuery(
    bool? OnlyParents = false) : IQuery<List<CategoryDetailDto>>;
