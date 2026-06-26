using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.CheckOrderNumberExistence
{
    public sealed class CheckOrderNumberExistenceHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<CheckOrderNumberExistenceQuery, bool>
    {
        public async Task<Result<bool>> Handle(
            CheckOrderNumberExistenceQuery query,
            CancellationToken cancellationToken)
        {
            Result<OrderId> excludeOrderId = null;
            if (query.ExcludeOrderId is not null)
            {
                var excludeOrderIdResult = OrderId.CreateFrom(query.ExcludeOrderId.Value);
                if (excludeOrderIdResult.IsFailure)
                    return excludeOrderIdResult.Error;
                excludeOrderId = excludeOrderIdResult;
            }

            var isOrderNumberExist = await orderQueryService.IsOrderNumberExistAsync(
                query.OrderNumber,
                excludeOrderId?.Value,
                cancellationToken);
            return (isOrderNumberExist);
        }
    }
}
