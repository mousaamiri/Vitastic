using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Commands.UpdateLevel
{
    public sealed record ChangeCourseLevelCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public CourseLevelDto Level { get; init; }

        public ChangeCourseLevelCommand() { }
    }

}
