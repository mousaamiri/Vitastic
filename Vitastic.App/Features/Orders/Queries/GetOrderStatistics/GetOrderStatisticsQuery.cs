using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.GetOrderStatistics
{
    public sealed record GetOrderStatisticsQuery : IQuery<OrderStatisticsDto>;
}
