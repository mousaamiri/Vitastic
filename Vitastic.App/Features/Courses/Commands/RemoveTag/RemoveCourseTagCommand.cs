using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.RemoveTag
{
    public sealed record RemoveCourseTagCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid TagId { get; init; }

        public RemoveCourseTagCommand() { }

        public RemoveCourseTagCommand(Guid courseId, Guid tagId)
        {
            CourseId = courseId;
            TagId = tagId;
        }
    }

}
