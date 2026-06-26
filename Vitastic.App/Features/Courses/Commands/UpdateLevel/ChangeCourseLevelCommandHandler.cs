using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.UpdateLevel
{
    public sealed class ChangeCourseLevelCommandHandler : ICommandHandler<ChangeCourseLevelCommand>
    {
        private readonly ICourseRepository _courseRepository;

        public ChangeCourseLevelCommandHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Result> Handle(ChangeCourseLevelCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await _courseRepository.FindAsync(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var result = course.ChangeLevel((CourseLevel)request.Level);
            if (result.IsFailure)
                return result.Error;

            return Result.Success();
        }
    }
}