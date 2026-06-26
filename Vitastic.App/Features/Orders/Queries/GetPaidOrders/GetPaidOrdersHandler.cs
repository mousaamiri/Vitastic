using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetPaidOrders
{
    public sealed class GetPaidOrdersHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetPaidOrdersQuery, PaginatedResult<OrderDto>>
    {
        public async Task<Result<PaginatedResult<OrderDto>>> Handle(
            GetPaidOrdersQuery query,
            CancellationToken cancellationToken)
        {
            (IReadOnlyList<OrderDto> Items, int Total)= await orderQueryService.GetPaidOrdersAsync(
                query.PageNumber,
                query.PageSize,
                cancellationToken);
            return new PaginatedResult<OrderDto>(Items, Total, query.PageNumber, query.PageSize);
        }
    }
}