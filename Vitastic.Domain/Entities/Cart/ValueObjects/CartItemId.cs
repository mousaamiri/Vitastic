using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Cart.ValueObjects;

public class CartItemId:GuidBasedId<CartItemId>
{
    //Constructor
    private CartItemId(Guid value) : base(value) { }
    //Override
    protected override CartItemId Create(Guid value) => new(value);
    public static CartItemId New() => new(Guid.NewGuid());
    public static Result<CartItemId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new CartItemId(guid), BaseIdErrors.Empty);
    public static Result<CartItemId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new CartItemId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
