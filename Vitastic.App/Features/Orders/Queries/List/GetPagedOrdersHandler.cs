using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.List
{
    public sealed class GetPagedOrdersHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetPagedOrdersQuery, PaginatedResult<OrderDto>>
    {
        public async Task<Result<PaginatedResult<OrderDto>>> Handle(
            GetPagedOrdersQuery query,
            CancellationToken cancellationToken)
        {
            (IReadOnlyList<OrderDto> items, int total) = await orderQueryService.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                query.Status,
                cancellationToken);
            return new PaginatedResult<OrderDto>(items, total, query.PageNumber, query.PageSize);
        }
    }
}