using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.GetRevenueStatistics
{
    public sealed record GetRevenueStatisticsQuery(
        DateTimeOffset?  FromDate = null,
        DateTimeOffset?  ToDate = null) : IQuery<RevenueStatisticsDto>;
}
