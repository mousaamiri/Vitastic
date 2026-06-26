using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.UpdateDetails;

public record UpdateCourseDetailsCommand : ICommand<Guid>
{
    public Guid CourseId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? ShortDescription { get; init; }

    public UpdateCourseDetailsCommand() { }
}


internal class UpdateCourseDetailsCommandHandler(ICourseRepository courseRepository)
    : ICommandHandler<UpdateCourseDetailsCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateCourseDetailsCommand command, CancellationToken ct)
    {
        var courseIdResult = CourseId.CreateFrom(command.CourseId);
        if (courseIdResult.IsFailure)
            return courseIdResult.Error;
        var course = await courseRepository.FindAsync(courseIdResult.Value, ct);
        if (course == null)
            return Result.Failure<Guid>(Error.NotFound("Course.NotFound", "دوره یافت نشد"));


        if (command.Title is not null)
        {
            var titleResult = CourseTitle.Create(command.Title);
            if(titleResult.IsFailure) return titleResult.Error;
            var setTitleResult =course.UpdateTitle(titleResult.Value);
            if(setTitleResult.IsFailure)
                return setTitleResult.Error;
        }

        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure) return descResult.Error;
            var setDescResult = course.UpdateDescription(descResult.Value);
            if(setDescResult.IsFailure) return setDescResult.Error;
        }


        if (command.ShortDescription is not null)
        {
            var shortDescResult = ShortDescription.Create(command.ShortDescription);
            if (shortDescResult.IsFailure) return shortDescResult.Error;
            var setShortDescResult = course.UpdateShortDescription(shortDescResult.Value);
            if(setShortDescResult.IsFailure) return setShortDescResult.Error;
        }

        await courseRepository.UpdateAsync(course, ct);

        return course.Id.Value;
    }
}
