using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Queries.GetBySlug
{
    public sealed record GetCourseBySlugQuery(string Slug, Guid? UserId = null, string? SessionId=null) : IQuery<CourseDto>;
}
