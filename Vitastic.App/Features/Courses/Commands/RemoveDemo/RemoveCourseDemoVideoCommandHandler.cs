using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.RemoveDemo
{
    public sealed class RemoveCourseDemoVideoCommandHandler(ICourseRepository courseRepository)
        : ICommandHandler<RemoveCourseDemoVideoCommand>
    {
        public async Task<Result> Handle(RemoveCourseDemoVideoCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await courseRepository.FindAsync(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var result = course.RemoveDemoVideo();
            if (result.IsFailure)
                return result.Error;
            await courseRepository.UpdateAsync(course,cancellationToken);
            return Result.Success();
        }
    }
}
