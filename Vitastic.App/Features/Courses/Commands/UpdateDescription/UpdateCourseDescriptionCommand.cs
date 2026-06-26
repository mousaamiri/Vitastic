using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.UpdateDescription
{
    public sealed record UpdateCourseDescriptionCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public string Description { get; init; }
        public string ShortDescription { get; init; }

        public UpdateCourseDescriptionCommand() { }
    }

}
