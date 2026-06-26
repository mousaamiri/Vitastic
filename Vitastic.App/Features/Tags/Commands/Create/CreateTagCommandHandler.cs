using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Tags.Dtos;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Tags.Commands.Create
{
    public sealed class CreateTagCommandHandler(ITagRepository tagRepository)
        : ICommandHandler<CreateTagCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateTagCommand command, CancellationToken cancellationToken)
        {
            Result<Tag> tagResult = Tag.Create(command.Name);
            if (tagResult.IsFailure)
                return tagResult.Error;
            Tag tag = tagResult.Value;
            bool isExistByName = await tagRepository.IsExistByNameAsync(tag.Name, cancellationToken);
            if (isExistByName)
                return Error.Conflict("CreateTagCommand.Conflict", "این برچسب از قبل موجود است.");
            await tagRepository.AddAsync(tag,cancellationToken);
            return tag.Id.Value;
        }
    }
}
