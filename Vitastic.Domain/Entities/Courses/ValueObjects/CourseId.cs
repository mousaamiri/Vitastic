using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class CourseId: GuidBasedId<CourseId>
{
    //Constructor
    private CourseId(Guid value) : base(value) { }
    //Override
    protected override CourseId Create(Guid value) => new(value);
    public static CourseId New() => new(Guid.NewGuid());
    public static Result<CourseId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new CourseId(guid), BaseIdErrors.Empty);
    public static Result<CourseId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new CourseId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}

