using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Discounts.ValueObjects;

public sealed class DiscountId : GuidBasedId<DiscountId>
{
    //Constructor
    private DiscountId(Guid value) : base(value) { }
    //Override
    protected override DiscountId Create(Guid value) => new(value);
    public static DiscountId New() => new(Guid.NewGuid());
    public static Result<DiscountId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new DiscountId(guid), BaseIdErrors.Empty);
    public static Result<DiscountId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new DiscountId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
