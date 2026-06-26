using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.RemoveSectionEpisode
{
    public sealed record RemoveCourseEpisodeCommand : ICommand
    {
        public Guid CourseId { get; init; }
        public Guid SectionId { get; init; }
        public Guid EpisodeId { get; init; }

        public RemoveCourseEpisodeCommand() { }

        public RemoveCourseEpisodeCommand(Guid courseId, Guid sectionId, Guid episodeId)
        {
            CourseId = courseId;
            SectionId = sectionId;
            EpisodeId = episodeId;
        }
    }

}
