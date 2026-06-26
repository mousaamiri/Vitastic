using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.GetPendingOrders
{
    public sealed record GetPendingOrdersQuery(int PageNumber, int PageSize)
        : IQuery<PaginatedResult<OrderDto>>;
}
