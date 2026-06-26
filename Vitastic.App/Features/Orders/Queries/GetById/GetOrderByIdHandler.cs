using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetById
{
    public sealed class GetOrderByIdHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetOrderByIdQuery, OrderDetailDto>
    {
        public async Task<Result<OrderDetailDto>> Handle(
            GetOrderByIdQuery query,
            CancellationToken cancellationToken)
        {
            Result<OrderId> orderIdResult = OrderId.CreateFrom(query.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            OrderDetailDto? orderDetailDto = await orderQueryService.GetByIdAsync(
                orderIdResult.Value,
                cancellationToken);
            if(orderDetailDto is null)
                return Error.NotFound("GetOrderByIdQuery.OrderNotFound","سفارش مورد نظر یافت نشد");
            return orderDetailDto;
        }
    }
}
