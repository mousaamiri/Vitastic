using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.Search
{
    public sealed class SearchOrdersHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<SearchOrdersQuery, PaginatedResult<OrderDto>>
    {
        public async Task<Result<PaginatedResult<OrderDto>>> Handle(
            SearchOrdersQuery query,
            CancellationToken cancellationToken)
        {
            (IReadOnlyList<OrderDto> items, int total)= await orderQueryService.SearchAsync(
                query.SearchTerm,
                query.PageNumber,
                query.PageSize,
                query.Status,
                cancellationToken);
            return new PaginatedResult<OrderDto>(items, total, query.PageNumber, query.PageSize);
        }
    }
}
