using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.Update;

public record UpdateCourseCommand(
    Guid CourseId,
    string CourseTitle,
    string Description,
    string ShortDescription,
    string Slug,
    CourseLevelDto CourseLevel,
    Guid InstructorId,
    List<SectionDto> Sections):ICommand;
public sealed class UpdateCourseCommandValidator: AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {

    }
}
public sealed class UpdateCourseCommandHandler:ICommandHandler<UpdateCourseCommand>
{
    public Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
