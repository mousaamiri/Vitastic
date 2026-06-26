using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Orders.ValueObjects;

public sealed class OrderId : GuidBasedId<OrderId>
{
    //Constructor
    private OrderId(Guid value) : base(value) { }
    //Override
    protected override OrderId Create(Guid value) => new(value);
    public static OrderId New() => new(Guid.NewGuid());
    public static Result<OrderId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new OrderId(guid), BaseIdErrors.Empty);
    public static Result<OrderId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new OrderId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
