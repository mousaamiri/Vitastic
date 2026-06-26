using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.ApplyDiscount
{
    public sealed class ApplyDiscountCommandHandler(IOrderRepository orderRepository, IDiscountRepository discountRepository) : ICommandHandler<ApplyDiscountCommand>
    {
        public async Task<Result> Handle(ApplyDiscountCommand command, CancellationToken cancellationToken)
        {
            var orderIdResult = OrderId.CreateFrom(command.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if (order is null)
                return Error.NotFound("ApplyDiscount.OrderNotFound", "سفارش یافت نشد.");

            var discountIdResult = DiscountId.CreateFrom(command.DiscountId);
            if (discountIdResult.IsFailure)
                return discountIdResult.Error;

            var discount = await discountRepository.FindAsync(discountIdResult.Value, cancellationToken);
            if (discount is null)
                return Error.NotFound("ApplyDiscount.DiscountNotFound", "کد تخفیف با این شناسه یافت نشد");
            var discountAmount = discount.CalculateDiscountAmount(order.ItemsTotal);
            if(discountAmount.IsFailure)
                return discountAmount.Error;
            var applyResult = order.ApplyDiscount(discount.Id,discount.Code,discountAmount.Value);

            if (applyResult.IsFailure)
                return applyResult.Error;

            await orderRepository.UpdateAsync(order, cancellationToken);
            return Result.Success();
        }
    }
}
