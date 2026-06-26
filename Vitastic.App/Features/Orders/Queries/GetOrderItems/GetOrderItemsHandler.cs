using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetOrderItems
{
    public sealed class GetOrderItemsHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetOrderItemsQuery, IReadOnlyList<OrderItemDto>>
    {
        public async Task<Result<IReadOnlyList<OrderItemDto>>> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
        {
            Result<OrderId> orderIdResult = OrderId.CreateFrom(request.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            var orders =  (
                await orderQueryService.GetOrderItemsAsync(
                    orderIdResult.Value,
                    cancellationToken)).ToList();
            return orders;
        }
    }
}
