using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.SetShippingAmount;

public sealed record SetShippingAmountCommand(
    Guid OrderId,
    decimal ShippingAmount) : ICommand;
