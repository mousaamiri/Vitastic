using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.ChangeInstructor
{
    public sealed record ChangeCourseInstructorCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid InstructorId { get; init; }

        public ChangeCourseInstructorCommand() { }
    }

}
