using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.UpdateTitle
{
    public sealed record UpdateCourseTitleCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public string Title { get; init; }

        public UpdateCourseTitleCommand() { }
    }

}
