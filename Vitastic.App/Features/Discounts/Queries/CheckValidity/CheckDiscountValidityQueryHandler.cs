using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Queries.CheckValidity
{
    public sealed class CheckDiscountValidityQueryHandler(
        IDiscountQueryService discountService)
        : IQueryHandler<CheckDiscountValidityQuery, bool>
    {
        public async Task<Result<bool>> Handle(
            CheckDiscountValidityQuery query,
            CancellationToken cancellationToken)
        {
            var discountIdResult = DiscountId.CreateFrom(query.DiscountId);
            if (discountIdResult.IsFailure)
                return discountIdResult.Error;

            return await discountService.IsValid(
                discountIdResult.Value,
                cancellationToken);
        }
    }
}
