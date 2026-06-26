using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Queries.GetUserOrdersCount
{
    public sealed record GetUserOrdersCountQuery(Guid UserId) : IQuery<int>;
}
