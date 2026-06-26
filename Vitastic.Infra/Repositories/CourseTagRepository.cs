using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Courses.ValueObjects;

namespace Vitastic.Infra.Repositories
{
    internal class CourseTagRepository(ApplicationWriteDbContext dbContext)
        : BaseRepository<CourseTag, CourseTagId>(dbContext), ICourseTagRepository
    {
        public async Task<bool> RelationIsExist(CourseTag relation, CancellationToken token = default)
            => await ExecuteAsync(
                async () => await
                    Context.CourseTags
                        .AnyAsync(r =>
                                (r.CourseId == relation.CourseId && r.TagId == relation.TagId)
                                || (r.Id == relation.Id)
                            , token), token);

        public async Task<bool> RelationIsExist(CourseId courseId, TagId tagId, CancellationToken token = default)
            => await ExecuteAsync(
                async () => await
                    Context.CourseTags
                        .AnyAsync(r => r.CourseId == courseId && r.TagId == tagId, token), token);

        public async Task<CourseTag?> FindAsync(CourseId courseId, TagId tagId, CancellationToken token = default)
            => await ExecuteAsync(
                async () => await
                    Context.CourseTags
                        .FirstOrDefaultAsync(r => r.CourseId == courseId && r.TagId == tagId, token), token);

        public async Task ReassignCourseTagsAsync(CourseId courseId, IEnumerable<TagId> tagIds,
            CancellationToken token = default)
        {
            await ExecuteAsync(async () =>
            {
                List<CourseTag> relationToRemove = await Context.CourseTags
                    .Where(cc => cc.CourseId == courseId).ToListAsync(token);
                Context.CourseTags.RemoveRange(relationToRemove);
                await Context.CourseTags.AddRangeAsync
                    (tagIds.Select(ci => CourseTag.Create(courseId, ci)), token);
            }, token);
        }
    }
}
