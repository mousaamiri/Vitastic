using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Cart.ValueObjects;

public class CartId:GuidBasedId<CartId>
{
    //Constructor
    private CartId(Guid value) : base(value) { }
    //Override
    protected override CartId Create(Guid value) => new(value);
    public static CartId New() => new(Guid.NewGuid());
    public static Result<CartId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new CartId(guid), BaseIdErrors.Empty);
    public static Result<CartId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new CartId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
