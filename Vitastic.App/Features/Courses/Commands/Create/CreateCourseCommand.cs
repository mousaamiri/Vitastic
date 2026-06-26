using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Commands.Create;

public sealed record CreateCourseCommand : ICommand<Guid>
{
    public string Title { get; init; }
    public string Description { get; init; }
    public string ShortDescription { get; init; }
    public string Slug { get; init; }
    public CourseLevelDto Level { get; init; }
    public Guid InstructorId { get; init; }
    public string? Currency { get; init; } = "IRT";

    public CreateCourseCommand() { }
}

