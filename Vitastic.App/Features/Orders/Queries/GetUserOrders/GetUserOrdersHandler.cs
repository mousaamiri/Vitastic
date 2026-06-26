using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetUserOrders
{
    public sealed class GetUserOrdersHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetUserOrdersQuery, PaginatedResult<OrderDto>>
    {
        public async Task<Result<PaginatedResult<OrderDto>>> Handle(
            GetUserOrdersQuery query,
            CancellationToken cancellationToken)
        {
            Result<UserId> userIdResult = UserId.CreateFrom(query.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            (IReadOnlyList<OrderDto> Items, int Total) = await orderQueryService.GetByUserIdAsync(
                userIdResult.Value,
                query.PageNumber,
                query.PageSize,
                (OrderStatus)query.Status!,
                cancellationToken);
            return new PaginatedResult<OrderDto>(Items, Total, query.PageNumber, query.PageSize);
        }
    }
}
