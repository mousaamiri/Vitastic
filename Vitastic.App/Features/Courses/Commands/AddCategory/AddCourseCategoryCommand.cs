using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.AddCategory
{
    public sealed record AddCourseCategoryCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid CategoryId { get; init; }

        public AddCourseCategoryCommand() { }
    }

}
