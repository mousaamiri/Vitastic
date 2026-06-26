using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories
{
    internal class CourseRepository(ApplicationWriteDbContext context) :
        BaseRepository<Course, CourseId>(context),
        ICourseRepository
    {
        public async Task<string> GetInstructorName(CourseId courseId, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(() => (
                from course in Context.Courses
                join instructor in Context.Instructors
                    on course.InstructorId equals instructor.Id
                where course.Id == courseId
                select instructor.FullName
            ).FirstOrDefaultAsync(cancellationToken), cancellationToken);
        }

        public async Task<bool> SectionTitleIsExistAsync(CourseId courseId, SectionTitle sectionTitle,
            CancellationToken token = default) =>
            await ExecuteAsync(async ()
                    => await Context.Sections.AnyAsync(s => s.CourseId == courseId && s.Title == sectionTitle, token),
                token);

        public async Task<bool> EpisodeTitleIsExistAsync(CourseId courseId, SectionId sectionId,
            EpisodeTitle episodeTitle,
            CancellationToken token = default)
            => await ExecuteAsync(async () =>
            {
                List<SectionId> courseSectionIds = await Context.Sections.Where(s => s.CourseId == courseId)
                    .Select(s => s.Id).ToListAsync(token);
                return await Context.Episodes
                    .AnyAsync(e =>
                            courseSectionIds.Contains(e.SectionId)
                            && e.Title == episodeTitle
                        , token);
            }, token);

        public async Task<Course?> GetCourseWithSectionAndEpisodes(CourseId courseId, CancellationToken token = default)
        {
            return await ExecuteAsync(async ()
                => await Context
                    .Courses
                    .Include(s => s.Sections)
                    .ThenInclude(s => s.Episodes)
                    .FirstOrDefaultAsync(c => c.Id == courseId, token), token);
        }

        public async Task<bool> SectionIsBelongToCourse(SectionId sectionId, CourseId courseId,
            CancellationToken token = default)
            => await ExecuteAsync(async ()
                => await Context
                    .Sections
                    .AnyAsync(c => c.Id == sectionId && c.CourseId == courseId, token), token);

        public async Task<List<Course>> GetByIdsAsync(List<CourseId> courseIds, CancellationToken token = default)
        {
            return await ExecuteAsync(async ()
                    => await Context
                        .Courses
                        .Include(s => s.Sections)
                        .ThenInclude(s => s.Episodes)
                        .Where(c => courseIds.Contains(c.Id))
                        .ToListAsync(token)
                , token);
        }

        public async Task<bool> ExistsBySlugAsync(Slug slug, CancellationToken token = default) =>
            await ExecuteAsync(
                async () => await Context.Courses.AnyAsync(c => c.Slug.Equals(slug), token)
                , token);

        public async Task<Course?> FindIncludeMembersAsync(CourseId courseId, CancellationToken token = default)
            => await ExecuteAsync(
                async () => await Context.Courses
                    .Include(c => c.Tags)
                    .Include(c => c.Categories)
                    .FirstOrDefaultAsync(c => c.Id.Equals(courseId), token), token);

        public async Task<CourseTitle?> GetCourseTitleByIdAsync(CourseId courseId, CancellationToken token = default)
            => await ExecuteAsync(async () =>
            {
                Course? course =await Context.Courses
                    .FirstOrDefaultAsync(c => c.Id.Equals(courseId), token);
                return course?.Title;
            }, token);
    }
}
