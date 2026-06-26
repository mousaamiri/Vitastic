using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.RemoveSection
{
    public sealed record RemoveCourseSectionCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid SectionId { get; init; }

        public RemoveCourseSectionCommand() { }

        public RemoveCourseSectionCommand(Guid courseId, Guid sectionId)
        {
            CourseId = courseId;
            SectionId = sectionId;
        }
    }

}
