using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Queries.GetCourseWithSections
{
    public sealed record GetCourseWithSectionsQuery(Guid CourseId, Guid? UserId = null, string? SessionId=null) : IQuery<CourseDto>;
}
