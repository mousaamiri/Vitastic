using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Queries.GetById;

public sealed record GetCourseByIdQuery(Guid CourseId, Guid? UserId = null, string? SessionId=null)
    : IQuery<CourseDto>;
