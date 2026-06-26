using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetLatestUserOrder
{
    public sealed class GetLatestUserOrderHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetLatestUserOrderQuery, OrderDto>
    {
        public async Task<Result<OrderDto>> Handle(
            GetLatestUserOrderQuery query,
            CancellationToken cancellationToken)
        {
            Result<UserId> userIdResult = UserId.CreateFrom(query.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            OrderDto? orderDto = await orderQueryService.GetLatestOrderByUserIdAsync(
                userIdResult.Value,
                cancellationToken);
            return orderDto is null ? Error.NotFound("GetLatestUserOrder.OrderNotFound","سفارشی برای کاربر مورد نظر یافت نشد.") : Result<OrderDto>.Success(orderDto);
        }
    }
}
