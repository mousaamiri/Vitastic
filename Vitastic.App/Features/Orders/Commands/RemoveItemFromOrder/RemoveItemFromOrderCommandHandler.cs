using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.RemoveItemFromOrder
{
    public sealed class RemoveItemFromOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<RemoveItemFromOrderCommand>
    {
        public async Task<Result> Handle(RemoveItemFromOrderCommand command, CancellationToken cancellationToken)
        {
            var orderIdResult = OrderId.CreateFrom(command.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            var orderItemIdResult = OrderItemId.CreateFrom(command.OrderItemId);
            if (orderItemIdResult.IsFailure)
                return orderItemIdResult.Error;
            var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if (order is null)
                return Error.NotFound("RemoveItemFromOrder.OrderNotFound", "سفارش یافت نشد.");

            var removeResult = order.RemoveItem(orderItemIdResult.Value);

            if (removeResult.IsFailure)
                throw new InvalidOperationException(removeResult.Error.Message);

            await orderRepository.UpdateAsync(order, cancellationToken);
            return Result.Success();
        }
    }
}