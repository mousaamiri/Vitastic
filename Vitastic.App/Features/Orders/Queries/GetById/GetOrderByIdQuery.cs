using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.GetById
{
    public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDetailDto>;
}
