using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.SetImage
{
    public sealed record SetCourseImageCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public IFile ImageFile { get; init; }

        public SetCourseImageCommand() { }
    }

}
