using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Categories.ValueObjects;

public class CourseCategoryId:GuidBasedId<CourseCategoryId>
{
    //Constructor
    private CourseCategoryId(Guid value) : base(value) { }
    //Override
    protected override CourseCategoryId Create(Guid value) => new(value);
    public static CourseCategoryId New() => new(Guid.NewGuid());
    public static Result<CourseCategoryId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new CourseCategoryId(guid), BaseIdErrors.Empty);
    public static Result<CourseCategoryId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new CourseCategoryId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
