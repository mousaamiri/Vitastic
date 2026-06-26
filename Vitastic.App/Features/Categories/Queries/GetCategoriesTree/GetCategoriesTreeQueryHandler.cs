using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Queries.GetCategoriesTree;

public sealed class GetCategoriesTreeQueryHandler(ICategoryQueryService queryService,IMapper mapper)
    : IQueryHandler<GetCategoriesTreeQuery,List<CategoryDetailDto>>
{
    public async Task<Result<List<CategoryDetailDto>>> Handle(GetCategoriesTreeQuery treeQuery,
        CancellationToken cancellationToken)
        => await queryService.GetListedAsync(treeQuery.OnlyParents, cancellationToken);
}
