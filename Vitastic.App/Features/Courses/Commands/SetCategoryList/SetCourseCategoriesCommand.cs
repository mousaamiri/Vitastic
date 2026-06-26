using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.SetCategoryList
{
    public sealed record SetCourseCategoriesCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public List<Guid> CategoryIds { get; init; }

        public SetCourseCategoriesCommand() { }
    }

}
