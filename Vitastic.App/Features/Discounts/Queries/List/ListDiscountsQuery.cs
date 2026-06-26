using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Discounts.Dtos;

namespace Vitastic.App.Features.Discounts.Queries.List;

public record ListDiscountsQuery(int PageNumber,int PageSize)
    : IQuery<PaginatedResult<DiscountDto>>;
