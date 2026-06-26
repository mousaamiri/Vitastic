using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Commands.Extend
{
    public sealed class ExtendDiscountEndDateCommandHandler(
        IDiscountRepository discountRepository,
        ILogger<ExtendDiscountEndDateCommandHandler> logger)
        : ICommandHandler<ExtendDiscountEndDateCommand>
    {
        public async Task<Result> Handle(
            ExtendDiscountEndDateCommand command,
            CancellationToken ct)
        {
            var discountIdResult = DiscountId.CreateFrom(command.DiscountId);
            if (discountIdResult.IsFailure)
                return discountIdResult.Error;
            var discount = await discountRepository.FindAsync(
                discountIdResult.Value, ct);

            if (discount is null)
                return Error.NotFound("Discount", "تخفیف مورد نظر یافت نشد");

            var result = discount.ExtendEndDate(command.NewEndDate);
            if (result.IsFailure)
                return result.Error;

            await discountRepository.UpdateAsync(discount, ct);

            logger.LogInformation(
                "Discount end date extended - DiscountId: {DiscountId}, NewEndDate: {NewEndDate}",
                command.DiscountId, command.NewEndDate);

            return Result.Success();
        }
    }
}