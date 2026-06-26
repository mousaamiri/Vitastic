using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Discounts.Commands.SetMinimumAmount
{
    public sealed class SetMinimumOrderAmountCommandHandler(
        IDiscountRepository discountRepository,
        ILogger<SetMinimumOrderAmountCommandHandler> logger)
        : ICommandHandler<SetMinimumOrderAmountCommand>
    {
        public async Task<Result> Handle(
            SetMinimumOrderAmountCommand command,
            CancellationToken ct)
        {
            var discountIdResult = DiscountId.CreateFrom(command.DiscountId);
            if (discountIdResult.IsFailure)
                return discountIdResult.Error;
            var discount = await discountRepository.FindAsync(
                discountIdResult.Value, ct);

            if (discount is null)
                return Error.NotFound("Discount", "تخفیف مورد نظر یافت نشد");

            var amountResult = Money.Create(command.Amount, command.Currency);
            if (amountResult.IsFailure)
                return amountResult.Error;

            var result = discount.SetMinimumOrderAmount(amountResult.Value);
            if (result.IsFailure)
                return result.Error;

            await discountRepository.UpdateAsync(discount, ct);

            logger.LogInformation(
                "Minimum order amount set - DiscountId: {DiscountId}, Amount: {Amount}",
                command.DiscountId, command.Amount);

            return Result.Success();
        }
    }
}