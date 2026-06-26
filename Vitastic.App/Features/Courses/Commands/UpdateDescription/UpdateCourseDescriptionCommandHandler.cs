using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vitastic.App.Features.Courses.Commands.UpdateDescription
{
    public sealed class UpdateCourseDescriptionCommandHandler(ICourseRepository courseRepository)
        : ICommandHandler<UpdateCourseDescriptionCommand>
    {
        public async Task<Result> Handle(UpdateCourseDescriptionCommand request, CancellationToken cancellationToken)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var course = await courseRepository.FindAsync(courseIdResult.Value, cancellationToken);

            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            var descResult = Description.Create(request.Description);
            if (descResult.IsFailure) return descResult.Error;
            var shortDescResult = ShortDescription.Create(request.ShortDescription);
            if (shortDescResult.IsFailure) return shortDescResult.Error;

            var result = course.UpdateDescriptionWithShortDescription(descResult.Value,
                shortDescResult.Value);
            if (result.IsFailure)
                return result.Error;
            await courseRepository.UpdateAsync(course, cancellationToken);
            return Result.Success();
        }
    }
}
