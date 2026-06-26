using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetPendingOrders
{
    public sealed class GetPendingOrdersHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetPendingOrdersQuery, PaginatedResult<OrderDto>>
    {
        public async Task<Result<PaginatedResult<OrderDto>>> Handle(
            GetPendingOrdersQuery query,
            CancellationToken cancellationToken)
        {
            (IReadOnlyList<OrderDto> Items, int Total)= await orderQueryService.GetPendingOrdersAsync(
                query.PageNumber,
                query.PageSize,
                cancellationToken);
            return new PaginatedResult<OrderDto>(Items, Total, query.PageNumber, query.PageSize);
        }
    }
}