using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Discounts.Dtos;

namespace Vitastic.App.Features.Discounts.Queries.GetByCode
{
    public sealed record GetDiscountByCodeQuery(
        string Code) : IQuery<DiscountDto>;
}
