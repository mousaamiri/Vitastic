using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.SetTagList
{
    public sealed record SetCourseTagsCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public List<Guid> TagIds { get; init; }

        public SetCourseTagsCommand() { }
    }

}
