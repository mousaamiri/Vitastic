using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Tags.Dtos;

namespace Vitastic.App.Features.Tags.Queries.Search;

public sealed record SearchTagsQuery(
    string? SearchTerm,
    int PageNumber,
    int PageSize) : IQuery<PaginatedResult<TagDto>>;
