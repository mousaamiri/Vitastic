using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.UpdateSlug
{
    public sealed record UpdateCourseSlugCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public string Slug { get; init; }

        public UpdateCourseSlugCommand() { }
    }

}
