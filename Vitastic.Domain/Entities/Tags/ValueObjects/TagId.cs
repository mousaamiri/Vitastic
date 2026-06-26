using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Tags.ValueObjects;

public class TagId:GuidBasedId<TagId>
{
    //Constructor
    private TagId(Guid value) : base(value) { }
    //Override
    protected override TagId Create(Guid value) => new(value);
    public static TagId New() => new(Guid.NewGuid());
    public static Result<TagId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new TagId(guid), BaseIdErrors.Empty);
    public static Result<TagId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new TagId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
