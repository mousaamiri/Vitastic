using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Discounts.Dtos;

namespace Vitastic.App.Features.Discounts.Queries.GetById;

public sealed record GetDiscountQuery(
    Guid DiscountId) : IQuery<DiscountDetailDto>;
