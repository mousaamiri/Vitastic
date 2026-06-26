using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.UpdateSectionEpisode
{
    public sealed record UpdateCourseEpisodeCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid SectionId { get; init; }
        public Guid EpisodeId { get; init; }
        public string? Title { get; init; } = null;
        public TimeSpan? Duration { get; init; } = null;
        public decimal? Price { get; init; } = null;
        public string? Currency { get; init; } = null;
        public IFile? VideoFile { get; init; } = null;

        public UpdateCourseEpisodeCommand() { }
    }

}
