using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.RemoveSection
{
    public sealed class RemoveCourseSectionCommandHandler(ICourseRepository courseRepository)
        : ICommandHandler<RemoveCourseSectionCommand>
    {
        public async Task<Result> Handle(RemoveCourseSectionCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await courseRepository.GetCourseWithSectionAndEpisodes(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var sectionIdResult = SectionId.CreateFrom(request.SectionId);
            if(sectionIdResult.IsFailure)
                return sectionIdResult.Error;

            var result = course.RemoveSection(sectionIdResult.Value);
            if (result.IsFailure)
                return result.Error;

            return Result.Success();
        }
    }
}
