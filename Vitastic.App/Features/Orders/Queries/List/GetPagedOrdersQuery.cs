using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.List
{
    public sealed record GetPagedOrdersQuery(
        int PageNumber,
        int PageSize,
        OrderStatusDto? Status = null) : IQuery<PaginatedResult<OrderDto>>;
}
