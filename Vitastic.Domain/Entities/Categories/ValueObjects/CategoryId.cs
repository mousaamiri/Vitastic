using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Categories.ValueObjects;

public class CategoryId:GuidBasedId<CategoryId>
{
    //Constructor
    private CategoryId(Guid value) : base(value) { }
    //Override
    protected override CategoryId Create(Guid value) => new(value);
    public static CategoryId New() => new(Guid.NewGuid());
    public static Result<CategoryId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new CategoryId(guid), BaseIdErrors.Empty);
    public static Result<CategoryId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new CategoryId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
