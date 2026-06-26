using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Queries.GetCourseWithSections
{
    public sealed class GetCourseWithSectionsQueryHandler(ICourseQueryService courseQueryService, IMapper mapper)
        : IQueryHandler<GetCourseWithSectionsQuery, CourseDto>
    {
        public async Task<Result<CourseDto>> Handle(GetCourseWithSectionsQuery request, CancellationToken cancellationToken)
        {

            UserId? userId = null;
            if (request.UserId != null)
            {
                var userIdResult =UserId.CreateFrom(request.UserId.Value);
                userId = userIdResult.Value;
            }
            var courseId = CourseId.CreateFrom(request.CourseId);
            if(courseId.IsFailure)
                return courseId.Error;
            CourseDto? course = await courseQueryService.GetWithSectionsByCourseIdAsync(courseId.Value,userId,request.SessionId, cancellationToken);
            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");
            return course;
        }
    }
}
