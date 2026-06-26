using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.ReorderSections
{
    public sealed record ReorderCourseSectionsCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public List<Guid> OrderedSectionIds { get; init; }

        public ReorderCourseSectionsCommand() { }
    }

}
