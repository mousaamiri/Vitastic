using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.Unpublish
{
    public sealed record UnpublishCourseCommand(
        Guid CourseId
    ) : ICommand;
}