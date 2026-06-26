using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.SetDemo
{
    public sealed record SetCourseDemoVideoCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public IFile VideoFile { get; init; }

        public SetCourseDemoVideoCommand(Guid courseId,IFile videoFile)
        {
            CourseId = courseId;
            VideoFile = videoFile;
        }

        public SetCourseDemoVideoCommand()
        {

        }
    }

}
