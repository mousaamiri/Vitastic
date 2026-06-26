using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.ReorderSections
{
    public sealed class ReorderCourseSectionsCommandHandler : ICommandHandler<ReorderCourseSectionsCommand>
    {
        private readonly ICourseRepository _courseRepository;

        public ReorderCourseSectionsCommandHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Result> Handle(ReorderCourseSectionsCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await _courseRepository.FindAsync(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var sectionIds = request.OrderedSectionIds
                .Select(id => SectionId.CreateFrom(id).Value)
                .ToList();

            var result = course.ReorderSections(sectionIds);
            if (result.IsFailure)
                return result.Error;

            return Result.Success();
        }
    }
}