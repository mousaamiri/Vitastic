using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Commands.AddSection
{
    public sealed record AddCourseSectionCommand: ICommand<SectionDto>
    {

        public Guid CourseId{get; init; }
        public string Title { get; init; } = null!;
        public int DisplayOrder{get; init; }

        public AddCourseSectionCommand() { }
    }

}
