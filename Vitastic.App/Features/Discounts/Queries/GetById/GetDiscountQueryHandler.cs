using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Queries.GetById
{
    public sealed class GetDiscountQueryHandler(
        IDiscountQueryService discountService)
        : IQueryHandler<GetDiscountQuery, DiscountDetailDto>
    {
        public async Task<Result<DiscountDetailDto>> Handle(
            GetDiscountQuery query,
            CancellationToken cancellationToken)
        {
            var discountIdResult = DiscountId.CreateFrom(query.DiscountId);
            if (discountIdResult.IsFailure)
                return discountIdResult.Error;

            var discount = await discountService.GetByIdAsync(
                discountIdResult.Value,
                cancellationToken);

            if (discount is null)
                return Error.NotFound("Discount", "تخفیف یافت نشد");

            return discount;
        }
    }
}