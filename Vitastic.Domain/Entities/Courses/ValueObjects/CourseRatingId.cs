using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class CourseRatingId: GuidBasedId<CourseRatingId>
{
    //Constructor
    private CourseRatingId(Guid value) : base(value) { }
    //Override
    protected override CourseRatingId Create(Guid value) => new(value);
    public static CourseRatingId New() => new(Guid.NewGuid());
    public static Result<CourseRatingId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new CourseRatingId(guid), BaseIdErrors.Empty);
    public static Result<CourseRatingId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new CourseRatingId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}

