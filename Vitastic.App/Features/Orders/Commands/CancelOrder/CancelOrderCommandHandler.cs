using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<CancelOrderCommand>
    {
        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var orderIdResult = OrderId.CreateFrom(request.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if (order is null)
                return Error.NotFound("CancelOrderCommand.OrderNotFound", "سفارشی با این شناسه یافت نشد. ");

            order.Cancel(request.CancelReason);

            await orderRepository.UpdateAsync(order, cancellationToken);

            return Result.Success();
        }
    }
}