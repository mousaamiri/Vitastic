using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Tags.Commands.Deactivate;

public sealed class DeactivateTagCommandHandler(ITagRepository tagRepository) : ICommandHandler<DeactivateTagCommand>
{
    public async Task<Result> Handle(DeactivateTagCommand command, CancellationToken cancellationToken)
    {
        Result<TagId> tagIdResult = TagId.CreateFrom(command.Id);
        if (tagIdResult.IsFailure)
            return tagIdResult.Error;
        Tag? tag =await tagRepository.FindAsync(tagIdResult.Value,cancellationToken);
        if (tag is null)
            return Error.NotFound("DeactivateTagCommand.TagNotFound", "هیچ برچسبی با این شناسه یافت نشد. ");
        tag.Deactivate();
        await tagRepository.UpdateAsync(tag,cancellationToken);
        return Result.Success();
    }
}
