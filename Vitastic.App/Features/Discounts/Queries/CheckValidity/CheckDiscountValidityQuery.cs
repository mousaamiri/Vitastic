using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Discounts.Queries.CheckValidity
{
    public sealed record CheckDiscountValidityQuery(
        Guid DiscountId,
        Guid? UserId = null) : IQuery<bool>;
}
