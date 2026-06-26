using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.App.Features.Common.Dtos;

namespace Vitastic.App.Features.Categories.Queries.Search;

public sealed record SearchCategoriesQuery(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 10,
    bool OnlyParents = false,
    bool OnlyActive = true):IQuery<PaginatedResult<CategoryListDto>>;
