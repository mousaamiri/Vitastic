using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.RemoveCategory
{
    public sealed record RemoveCourseCategoryCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid CategoryId { get; init; }

        public RemoveCourseCategoryCommand() { }

        public RemoveCourseCategoryCommand(Guid courseId, Guid categoryId)
        {
            CourseId = courseId;
            CategoryId = categoryId;
        }
    }

}
