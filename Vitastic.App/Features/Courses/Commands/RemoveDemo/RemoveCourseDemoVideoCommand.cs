using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.RemoveDemo
{
    public sealed record RemoveCourseDemoVideoCommand(
        Guid CourseId
    ) : ICommand;
}