using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Tags.Commands.Update;

public sealed class UpdateTagCommandHandler(ITagRepository tagRepository) : ICommandHandler<UpdateTagCommand>
{
    public async Task<Result> Handle(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        Result<TagId> tagIdResult = TagId.CreateFrom(command.Id);
        if (tagIdResult.IsFailure)
            return tagIdResult.Error;
        Tag? tag = await tagRepository.FindAsync(tagIdResult.Value, cancellationToken);
        if (tag is null)
            return Error.NotFound("UpdateTagCommand.TagNotFound", "برچسب با این شناسه یافت نشد.");
        tag.UpdateName(command.Name);
        await tagRepository.UpdateAsync(tag, cancellationToken);
        return Result.Success();
    }
}
