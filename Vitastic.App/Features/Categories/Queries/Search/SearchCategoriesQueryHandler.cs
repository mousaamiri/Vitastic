using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Queries.Search;

public sealed class SearchCategoriesQueryHandler(ICategoryQueryService queryService,IMapper mapper)
    : IQueryHandler<SearchCategoriesQuery, PaginatedResult<CategoryListDto>>
{
    public async Task<Result<PaginatedResult<CategoryListDto>>> Handle(SearchCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        (IReadOnlyList<CategoryListDto> items, var total) =
            await queryService.SearchAsync(query.SearchTerm, query.PageNumber, query.PageSize, query.OnlyParents,query.OnlyActive ,cancellationToken);
        return new PaginatedResult<CategoryListDto>(items, total, query.PageNumber, query.PageSize);
    }
}
