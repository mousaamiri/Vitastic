using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.ApplyDiscount;

public sealed record ApplyDiscountCommand(
    Guid OrderId,
    Guid DiscountId) : ICommand;
