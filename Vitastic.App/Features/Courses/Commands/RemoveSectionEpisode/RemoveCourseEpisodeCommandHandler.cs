using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.RemoveSectionEpisode
{
    public sealed class RemoveCourseEpisodeCommandHandler(ICourseRepository courseRepository)
        : ICommandHandler<RemoveCourseEpisodeCommand>
    {
        public async Task<Result> Handle(RemoveCourseEpisodeCommand request, CancellationToken cancellationToken)
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
            var episodeId = EpisodeId.CreateFrom(request.EpisodeId);
            if(episodeId.IsFailure)
                return episodeId.Error;

            var result = course.RemoveEpisode(sectionId.Value, episodeId.Value);
            if (result.IsFailure)
                return result.Error;

            return Result.Success();
        }
    }
}
