using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Tags.Dtos;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Tags.Queries.List;

public sealed class ListTagsQueryHandler(ITagQueryService tagService,IMapper mapper)
    : IQueryHandler<ListTagsQuery, PaginatedResult<TagDto>>
{
    public async Task<Result<PaginatedResult<TagDto>>> Handle(ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        (IReadOnlyList<TagDto> items, var total) = await tagService.GetPagedAsync(query.PageNumber, query.PageSize, true,cancellationToken);
        return  new PaginatedResult<TagDto>(items, total, query.PageNumber, query.PageSize);
    }
}
