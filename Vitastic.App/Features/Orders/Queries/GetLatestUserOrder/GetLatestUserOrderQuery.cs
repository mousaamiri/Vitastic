using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.GetLatestUserOrder
{
    public sealed record GetLatestUserOrderQuery(Guid UserId) : IQuery<OrderDto>;
}
