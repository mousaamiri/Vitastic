using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetUserOrdersCount
{
    public sealed class GetUserOrdersCountHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetUserOrdersCountQuery, int>
    {
        public async Task<Result<int>> Handle(
            GetUserOrdersCountQuery query,
            CancellationToken cancellationToken)
        {
            Result<UserId> userIdResult = UserId.CreateFrom(query.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            return await orderQueryService.GetUserOrdersCountAsync(
                userIdResult.Value,
                cancellationToken);
        }
    }
}
