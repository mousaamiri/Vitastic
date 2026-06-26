using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.RemoveDiscount;

public sealed record RemoveDiscountCommand(
    Guid OrderId) : ICommand;
