using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Tags.Dtos;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Tags.Queries.GetById;

public sealed class GetTagByIdQueryHandler(ITagQueryService tagservice,IMapper mapper) : IQueryHandler<GetTagByIdQuery, TagDto>
{
    public async Task<Result<TagDto>> Handle(GetTagByIdQuery command, CancellationToken cancellationToken)
    {
        Result<TagId> tagIdResult = TagId.CreateFrom(command.Id);
        if (tagIdResult.IsFailure)
            return tagIdResult.Error;
        TagDto? tag = await tagservice.GetDetailAsync(tagIdResult.Value, cancellationToken);
        if (tag is null)
            return Error.NotFound("GetTagByIdQuery.TagNotFound", "برچسبی با این شناسه یافت نشد.");
        return tag;
    }
}
