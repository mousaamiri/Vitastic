using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Commands.Deactivate
{
    public sealed class DeactivateDiscountCommandHandler(
        IDiscountRepository discountRepository,
        ILogger<DeactivateDiscountCommandHandler> logger)
        : ICommandHandler<DeactivateDiscountCommand>
    {
        public async Task<Result> Handle(
            DeactivateDiscountCommand command,
            CancellationToken ct)
        {
            var discountIdResult = DiscountId.CreateFrom(command.DiscountId);
            if (discountIdResult.IsFailure)
                return discountIdResult.Error;
            var discount = await discountRepository.FindAsync(
                discountIdResult.Value, ct);

            if (discount is null)
                return Error.NotFound("Discount", "تخفیف مورد نظر یافت نشد");

            var result = discount.Deactivate();
            if (result.IsFailure)
                return result.Error;

            await discountRepository.UpdateAsync(discount, ct);

            logger.LogInformation(
                "Discount deactivated - DiscountId: {DiscountId}",
                command.DiscountId);

            return Result.Success();
        }
    }
}