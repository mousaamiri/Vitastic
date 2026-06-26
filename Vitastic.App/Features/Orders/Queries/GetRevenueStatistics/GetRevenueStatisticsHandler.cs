using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetRevenueStatistics
{
    public sealed class GetRevenueStatisticsHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetRevenueStatisticsQuery, RevenueStatisticsDto>
    {
        public async Task<Result<RevenueStatisticsDto>> Handle(
            GetRevenueStatisticsQuery query,
            CancellationToken cancellationToken)
        {
            return await orderQueryService.GetRevenueStatisticsAsync(
                query.FromDate,
                query.ToDate,
                cancellationToken);
        }
    }
}