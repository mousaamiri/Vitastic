using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.Archive
{
    public sealed record ArchiveCourseCommand(
        Guid CourseId
    ) : ICommand;
}