using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Orders.ValueObjects;

public sealed class OrderNoteId:GuidBasedId<OrderNoteId>
{
    //Constructor
    private OrderNoteId(Guid value) : base(value) { }
    //Override
    protected override OrderNoteId Create(Guid value) => new(value);
    public static OrderNoteId New() => new(Guid.NewGuid());
    public static Result<OrderNoteId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new OrderNoteId(guid), BaseIdErrors.Empty);
    public static Result<OrderNoteId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new OrderNoteId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
