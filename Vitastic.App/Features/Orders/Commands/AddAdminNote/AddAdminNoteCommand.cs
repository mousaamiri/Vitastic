using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.AddAdminNote;

public sealed record AddAdminNoteCommand(
    Guid OrderId,
    string Note) : ICommand;
