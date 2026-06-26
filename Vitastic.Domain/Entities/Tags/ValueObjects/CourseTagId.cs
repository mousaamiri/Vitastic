using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Tags.ValueObjects;

public class CourseTagId:GuidBasedId<CourseTagId>
{
    //Constructor
    private CourseTagId(Guid value) : base(value) { }
    //Override
    protected override CourseTagId Create(Guid value) => new(value);
    public static CourseTagId New() => new(Guid.NewGuid());
    public static Result<CourseTagId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new CourseTagId(guid), BaseIdErrors.Empty);
    public static Result<CourseTagId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new CourseTagId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
