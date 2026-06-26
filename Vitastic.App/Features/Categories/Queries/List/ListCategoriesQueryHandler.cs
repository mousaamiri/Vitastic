using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Queries.List;

public sealed class ListCategoriesQueryHandler(ICategoryQueryService queryService,IMapper mapper)
    : IQueryHandler<ListCategoriesQuery,PaginatedResult<CategoryListDto>>
{
    public async Task<Result<PaginatedResult<CategoryListDto>>> Handle(ListCategoriesQuery treeQuery,
        CancellationToken cancellationToken)
    {
        (IReadOnlyCollection<CategoryListDto> items, var count)  =
            await queryService.GetPagedAsync(treeQuery.PageSize,treeQuery.PageNumber,treeQuery.OnlyParents, cancellationToken);
        return new PaginatedResult<CategoryListDto>(items, count, treeQuery.PageNumber, treeQuery.PageSize);
    }
}
