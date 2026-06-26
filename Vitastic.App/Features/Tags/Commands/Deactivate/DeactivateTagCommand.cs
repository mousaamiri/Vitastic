using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Tags.Commands.Deactivate;

public record DeactivateTagCommand(Guid Id) : ICommand;
