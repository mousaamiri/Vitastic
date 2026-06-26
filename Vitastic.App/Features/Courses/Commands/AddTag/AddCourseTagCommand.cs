using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.AddTag
{
    public sealed record AddCourseTagCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid TagId { get; init; }

        public AddCourseTagCommand() { }
    }

}
