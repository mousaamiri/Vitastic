using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Tags.Dtos;
using Vitastic.Domain.Entities.Tags;

namespace Vitastic.App.Features.Tags.Commands.Create;

public record CreateTagCommand(string Name) : ICommand<Guid>;
