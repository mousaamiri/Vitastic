using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders;

namespace Vitastic.App.Features.Orders.Commands.RemoveItemFromOrder;

public sealed record RemoveItemFromOrderCommand(
    Guid OrderId,
    Guid OrderItemId) : ICommand;
