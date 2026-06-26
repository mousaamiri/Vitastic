using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Discounts.Dtos;

namespace Vitastic.App.Features.Discounts.Queries.Calculate
{
    public sealed record CalculateDiscountAmountQuery(
        Guid UserId,
        string  DiscountCode,
        decimal OrderTotal,
        string? Currency="IRT") : IQuery<DiscountCalculationDto>;
}
