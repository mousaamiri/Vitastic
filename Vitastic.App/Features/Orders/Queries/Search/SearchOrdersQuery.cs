using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.App.Features.Orders.Queries.Search
{
    public sealed record SearchOrdersQuery(
        string SearchTerm,
        int PageNumber,
        int PageSize,
        OrderStatusDto? Status = null) : IQuery<PaginatedResult<OrderDto>>;

}
