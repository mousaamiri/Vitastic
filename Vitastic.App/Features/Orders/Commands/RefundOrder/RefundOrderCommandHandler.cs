using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.RefundOrder
{
    public sealed class RefundOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<RefundOrderCommand>
    {
        public async Task<Result> Handle(RefundOrderCommand request, CancellationToken cancellationToken)
        {
            //Get order by id
            var orderIdResult = OrderId.CreateFrom(request.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            //Get order 
            Order? order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if(order is null)
                return Error.NotFound("RefundOrderCommand.OrderNotFound", "سفارشی با این شناسه یافت نشد. ");
            //Refuound
            order.Refund(request.RefundReason);
            //Update 
            await orderRepository.UpdateAsync(order,cancellationToken);

            return Result.Success();
        }
    }
}