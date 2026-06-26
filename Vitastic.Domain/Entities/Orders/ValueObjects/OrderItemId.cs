using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Orders.ValueObjects;

public sealed class OrderItemId:GuidBasedId<OrderItemId>
{
    //Constructor
    private OrderItemId(Guid value) : base(value) { }
    //Override
    protected override OrderItemId Create(Guid value) => new(value);
    public static OrderItemId New() => new(Guid.NewGuid());
    public static Result<OrderItemId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new OrderItemId(guid), BaseIdErrors.Empty);
    public static Result<OrderItemId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new OrderItemId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
