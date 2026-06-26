using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.ChangeInstructor
{
    public sealed class ChangeCourseInstructorCommandHandler : ICommandHandler<ChangeCourseInstructorCommand>
    {
        private readonly ICourseRepository _courseRepository;

        public ChangeCourseInstructorCommandHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Result> Handle(ChangeCourseInstructorCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await _courseRepository.FindAsync(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var instructorIdResult = InstructorId.CreateFrom(request.InstructorId);
            if(instructorIdResult.IsFailure)
                return instructorIdResult.Error;

            var result = course.ChangeInstructor(instructorIdResult.Value);
            if (result.IsFailure)
                return result.Error;

            return Result.Success();
        }
    }
}