using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetOrderStatistics
{
    public sealed class GetOrderStatisticsHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetOrderStatisticsQuery, OrderStatisticsDto>
    {
        public async Task<Result<OrderStatisticsDto>> Handle(
            GetOrderStatisticsQuery query,
            CancellationToken cancellationToken)
        {
            return await orderQueryService.GetStatisticsAsync(cancellationToken);
        }
    }
}