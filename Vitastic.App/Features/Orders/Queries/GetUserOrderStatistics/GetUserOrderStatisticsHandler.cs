using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Queries.GetUserOrderStatistics
{
    public sealed class GetUserOrderStatisticsHandler(IOrderQueryService orderQueryService)
        : IQueryHandler<GetUserOrderStatisticsQuery, UserOrderStatisticsDto>
    {
        public async Task<Result<UserOrderStatisticsDto>> Handle(
            GetUserOrderStatisticsQuery query,
            CancellationToken cancellationToken)
        {
            Result<UserId> userIdResult = UserId.CreateFrom(query.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            return await orderQueryService.GetUserStatisticsAsync(
                userIdResult.Value,
                cancellationToken);
        }
    }
}
