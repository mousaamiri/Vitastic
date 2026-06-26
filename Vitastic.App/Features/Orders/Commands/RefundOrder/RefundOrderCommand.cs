using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.RefundOrder;

public sealed record RefundOrderCommand(
    Guid OrderId,
    string? RefundReason = null) : ICommand;
