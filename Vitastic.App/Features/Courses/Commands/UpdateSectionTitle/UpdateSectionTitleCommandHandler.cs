using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.UpdateSectionTitle
{
    public sealed class UpdateSectionTitleCommandHandler(ICourseRepository courseRepository)
        : ICommandHandler<UpdateSectionTitleCommand>
    {
        public async Task<Result> Handle(UpdateSectionTitleCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await courseRepository.GetCourseWithSectionAndEpisodes(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var sectionId = SectionId.CreateFrom(request.SectionId);
            if(sectionId.IsFailure)
                return sectionId.Error;
            var titleResult = SectionTitle.Create(request.Title);

            if (titleResult.IsFailure)
                return titleResult.Error;

            var result = course.UpdateSectionTitle(sectionId.Value, titleResult.Value);
            if (result.IsFailure)
                return result.Error;
            await courseRepository.UpdateAsync(course, cancellationToken);
            return Result.Success();
        }
    }
}
