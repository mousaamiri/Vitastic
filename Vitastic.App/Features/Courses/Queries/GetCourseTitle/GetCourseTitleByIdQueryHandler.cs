using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Queries.GetCourseTitle
{
    public sealed class GetCourseTitleByIdQueryHandler(ICourseRepository courseRepository):IQueryHandler<GetCourseTitleByIdQuery,string>
    {
        public async Task<Result<string>> Handle(GetCourseTitleByIdQuery request, CancellationToken cancellationToken)
        {
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure) return courseIdResult.Error;
            CourseTitle? courseTitle = await courseRepository.GetCourseTitleByIdAsync(courseIdResult.Value, cancellationToken);
            return courseTitle is null 
                ? Error.NotFound("GetCourseTitleByIdQueryHandler.CourseNotFound", "دوره ای با این شناسه یافت نشد.")
                : Result.Success(courseTitle.Value);
        }
    }
}