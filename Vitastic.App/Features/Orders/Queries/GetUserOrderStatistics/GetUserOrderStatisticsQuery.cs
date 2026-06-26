using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.GetUserOrderStatistics
{
    public sealed record GetUserOrderStatisticsQuery(Guid UserId) : IQuery<UserOrderStatisticsDto>;
}
