using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Tags.Dtos;

namespace Vitastic.App.Features.Tags.Queries.List;

public record ListTagsQuery(int PageNumber,int PageSize)
    : IQuery<PaginatedResult<TagDto>>;
