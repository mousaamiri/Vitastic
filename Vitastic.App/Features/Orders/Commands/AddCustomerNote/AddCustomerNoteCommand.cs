using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.AddCustomerNote;

public sealed record AddCustomerNoteCommand(
    Guid OrderId,
    string Note) : ICommand;
