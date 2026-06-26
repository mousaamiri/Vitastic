using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Orders.Commands.SetShippingAmount
{
    public sealed class SetShippingAmountCommandHandler(IOrderRepository orderRepository) : ICommandHandler<SetShippingAmountCommand>
    {
        public async Task<Result> Handle(SetShippingAmountCommand request, CancellationToken cancellationToken)
        {
            var orderIdResult = OrderId.CreateFrom(request.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if (order is null)
                return Error.NotFound("SetShippingAmountCommand.OrderNotFound","سفارشی با این شناسه یافت نشد. ");

            var amountResult = Money.Create(request.ShippingAmount);
            if (amountResult.IsFailure)
                return amountResult.Error;
            var result = order.SetShippingAmount(amountResult.Value);
            if (result.IsFailure)
                return result.Error;
            await orderRepository.UpdateAsync(order, cancellationToken);

            return Result.Success();
        }
    }
}
