using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Tags.Commands.Update;

public record UpdateTagCommand(Guid Id, string Name) : ICommand;
