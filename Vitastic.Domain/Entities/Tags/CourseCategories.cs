using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Domain.Entities.Tags;

public class CourseTag:BaseEntity<CourseTagId>
{
    public CourseId CourseId { get; private set; }
    public TagId TagId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }=DateTimeOffset.UtcNow;

    public CourseTag() { }//For ef
    private CourseTag(CourseTagId id, CourseId courseId, TagId tagId)
        : base(id)
    {
        CourseId = courseId;
        TagId = tagId;
    }
    public static CourseTag Create(CourseId courseId, TagId tagId)
    {
        return new CourseTag(CourseTagId.New(), courseId, tagId);
    }
}

