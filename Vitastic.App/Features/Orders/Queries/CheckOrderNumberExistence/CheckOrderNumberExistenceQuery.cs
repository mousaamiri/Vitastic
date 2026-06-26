using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Queries.CheckOrderNumberExistence
{
    public sealed record CheckOrderNumberExistenceQuery(
        string OrderNumber,
        Guid? ExcludeOrderId = null) : IQuery<bool>;
}
