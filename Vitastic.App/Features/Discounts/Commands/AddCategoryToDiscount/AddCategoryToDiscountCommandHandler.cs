using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Commands.AddCategoryToDiscount
{
    public sealed class AddCategoryToDiscountCommandHandler(
        IDiscountRepository discountRepository,
        ILogger<AddCategoryToDiscountCommandHandler> logger)
        : ICommandHandler<AddCategoryToDiscountCommand>
    {
        public async Task<Result> Handle(
            AddCategoryToDiscountCommand command,
            CancellationToken ct)
        {
            var discountIdResult = DiscountId.CreateFrom(command.DiscountId);
            if (discountIdResult.IsFailure)
                return discountIdResult.Error;
            var discount = await discountRepository.FindAsync(
                discountIdResult.Value, ct);

            if (discount is null)
                return Error.NotFound("Discount", "تخفیف مورد نظر یافت نشد");

            var categoryIdResult = CategoryId.CreateFrom(command.CategoryId);
            if (categoryIdResult.IsFailure)
                return categoryIdResult.Error;
            var result = discount.AddCategory(categoryIdResult.Value);
            if (result.IsFailure)
                return result.Error;

            await discountRepository.UpdateAsync(discount, ct);

            logger.LogInformation(
                "Category added to discount - DiscountId: {DiscountId}, CategoryId: {CategoryId}",
                command.DiscountId, command.CategoryId);

            return Result.Success();
        }
    }
}