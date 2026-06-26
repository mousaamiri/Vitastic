using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Discounts.Queries.Calculate
{
    public sealed class CalculateDiscountAmountQueryHandler(
        IDiscountQueryService discountService,
        IDiscountRepository discountRepository)
        : IQueryHandler<CalculateDiscountAmountQuery, DiscountCalculationDto>
    {
        public async Task<Result<DiscountCalculationDto>> Handle(
            CalculateDiscountAmountQuery query,
            CancellationToken cancellationToken)
        {
            Result<DiscountCode> discountCode = DiscountCode.Create(query.DiscountCode);
            if (discountCode.IsFailure)
                return discountCode.Error;
            Result<Currency> currencyResult = Currency.FromCode(query.Currency);
            if (currencyResult.IsFailure)
                return currencyResult.Error;
            Discount? discount = await discountRepository.GetByCodeAsync(discountCode.Value,cancellationToken);
            if(discount is null)
                return Error.NotFound("CalculateDiscountAmountQuery.DiscountNotFound","چنین کد تخفیف یافت نشد.");

            Result<bool> isValid =  await discountService.IsValid(discount.Id, cancellationToken);
            if (isValid.IsFailure)
                return isValid.Error;
            if(!isValid.Value)
                return Error.NotFound("CalculateDiscountAmountQuery.IsNotValid","کد تخفیف معتبر نیست.");

            return await discountService.CalculateDiscountAsync(
                discount.Id,
                query.OrderTotal,
                currencyResult.Value, cancellationToken);
        }
    }
}
