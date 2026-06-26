using System.Security.Cryptography;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface ICourseRepository:IRepository<Course,CourseId>
{
    Task<string> GetInstructorName(CourseId courseId, CancellationToken cancellationToken = default);
    Task<bool> SectionTitleIsExistAsync(CourseId courseId, SectionTitle sectionTitle, CancellationToken token = default);
    Task<bool> EpisodeTitleIsExistAsync(CourseId courseId, SectionId sectionId, EpisodeTitle episodeTitle, CancellationToken token = default);
    Task<Course?> GetCourseWithSectionAndEpisodes(CourseId courseId, CancellationToken token=default);
    Task<bool> SectionIsBelongToCourse(SectionId sectionId, CourseId courseId, CancellationToken token=default);
    Task<List<Course>> GetByIdsAsync(List<CourseId> courseIds, CancellationToken token = default);
    Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken token=default);
    Task<Course?> FindIncludeMembersAsync(CourseId courseId, CancellationToken token=default);
    Task<CourseTitle?> GetCourseTitleByIdAsync(CourseId courseId, CancellationToken token=default);
}
