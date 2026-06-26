using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.RemoveDiscount
{
    public sealed class RemoveDiscountCommandHandler(IOrderRepository orderRepository) : ICommandHandler<RemoveDiscountCommand>
    {
        public async Task<Result> Handle(RemoveDiscountCommand request, CancellationToken cancellationToken)
        {
            var orderIdResult = OrderId.CreateFrom(request.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if (order is null)
                return Error.NotFound("RemoveDiscountCommand.OrderNotFound","سفارشی با این شناسه یافت نشد. ");

            order.RemoveDiscount();
            await orderRepository.UpdateAsync(order, cancellationToken);

            return Result.Success();
        }
    }
}