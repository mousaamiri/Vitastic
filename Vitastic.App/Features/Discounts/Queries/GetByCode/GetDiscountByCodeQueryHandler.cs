using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Queries.GetByCode
{
    public sealed class GetDiscountByCodeQueryHandler(
        IDiscountQueryService discountService)
        : IQueryHandler<GetDiscountByCodeQuery, DiscountDto>
    {
        public async Task<Result<DiscountDto>> Handle(
            GetDiscountByCodeQuery query,
            CancellationToken cancellationToken)
        {
            var discountCodeResult = DiscountCode.Create(query.Code);
            if (discountCodeResult.IsFailure)
                return discountCodeResult.Error;

            var discount = await discountService.GetByCodeAsync(
                query.Code,
                cancellationToken);

            if (discount is null)
                return Error.NotFound("Discount", "تخفیف با این کد یافت نشد");

            return discount;
        }
    }
}