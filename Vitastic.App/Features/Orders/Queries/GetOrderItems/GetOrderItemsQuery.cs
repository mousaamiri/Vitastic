using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.GetOrderItems
{
    public sealed record GetOrderItemsQuery(Guid OrderId) : IQuery<IReadOnlyList<OrderItemDto>>;
}
