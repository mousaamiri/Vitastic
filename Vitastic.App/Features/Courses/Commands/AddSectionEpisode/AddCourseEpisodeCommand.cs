using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Commands.AddSectionEpisode
{
    public sealed record AddCourseEpisodeCommand : ICommand<EpisodeDto>
    {
        public Guid CourseId{get; init; }
        public Guid SectionId{get; init; }
        public string Title{get; init; }
        public TimeSpan Duration{get; init; }
        public decimal Price{get; init; }
        public string? Currency{get; init; }
        public IFile VideoFile { get; init; }
        public AddCourseEpisodeCommand() { }
    }
}
