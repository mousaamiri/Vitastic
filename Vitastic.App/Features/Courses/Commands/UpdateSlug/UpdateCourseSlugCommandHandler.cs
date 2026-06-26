using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.UpdateSlug
{
    public sealed class UpdateCourseSlugCommandHandler(ICourseRepository courseRepository)
        : ICommandHandler<UpdateCourseSlugCommand>
    {
        public async Task<Result> Handle(UpdateCourseSlugCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await courseRepository.GetCourseWithSectionAndEpisodes(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var slugResult = Slug.Create(request.Slug);
            if (slugResult.IsFailure)
                return slugResult.Error;

            var result = course.UpdateSlug(slugResult.Value);
            if (result.IsFailure)
                return result.Error;
            await courseRepository.UpdateAsync(course, cancellationToken);
            return Result.Success();
        }
    }
}
