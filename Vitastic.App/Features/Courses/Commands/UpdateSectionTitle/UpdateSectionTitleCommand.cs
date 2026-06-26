using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.UpdateSectionTitle
{
    public sealed record UpdateSectionTitleCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid SectionId { get; init; }
        public string Title { get; init; }

        public UpdateSectionTitleCommand() { }
    }

}
