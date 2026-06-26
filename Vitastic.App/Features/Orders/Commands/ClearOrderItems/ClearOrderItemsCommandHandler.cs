using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.ClearOrderItems
{
    public sealed class ClearOrderItemsCommandHandler(IOrderRepository orderRepository) : ICommandHandler<ClearOrderItemsCommand>
    {
        public async Task<Result> Handle(ClearOrderItemsCommand command, CancellationToken cancellationToken)
        {
            var orderIdResult = OrderId.CreateFrom(command.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if (order is null)
                return Error.NotFound("ClearOrderItems.OrderNotFound", "سفارش یافت نشد.");

            var clearResult = order.ClearItems();

            if (clearResult.IsFailure)
                throw new InvalidOperationException(clearResult.Error.Message);

            await orderRepository.UpdateAsync(order, cancellationToken);
            return Result.Success();
        }

    }
}