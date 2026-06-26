using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Queries.GetCourseTitle;

public record GetCourseTitleByIdQuery(Guid CourseId):IQuery<string>;
