using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Courses.ValueObjects;

namespace Vitastic.Infra.Repositories
{
    internal class CourseCategoryRepository(ApplicationWriteDbContext dbContext)
        : BaseRepository<CourseCategory, CourseCategoryId>(dbContext), ICourseCategoryRepository
    {
        public async Task<bool> RelationIsExist(CourseCategory relation, CancellationToken token = default)
            => await ExecuteAsync(
                async () => await
                    Context.CourseCategories
                        .AnyAsync(r =>
                                (r.CourseId == relation.CourseId && r.CategoryId == relation.CategoryId)
                                || (r.Id == relation.Id)
                            , token), token);

        public async Task<bool> RelationIsExist(CourseId courseId, CategoryId categoryId,
            CancellationToken token = default)
            => await ExecuteAsync(
                async () => await
                    Context.CourseCategories
                        .AnyAsync(r => r.CategoryId == categoryId && r.CourseId == courseId
                            , token), token);

        public async Task<CourseCategory?> FindAsync(CourseId courseId, CategoryId categoryId,
            CancellationToken token = default)
            => await ExecuteAsync(
                async () => await
                    Context.CourseCategories
                        .FirstOrDefaultAsync(r => r.CategoryId == categoryId && r.CourseId == courseId
                            , token), token);

        public async Task ReassignCourseCategoriesAsync(CourseId courseId, IEnumerable<CategoryId> categoryIds,
            CancellationToken token = default)
        {
            await ExecuteAsync(async () =>
            {
                List<CourseCategory> relationToRemove = await Context.CourseCategories
                    .Where(cc => cc.CourseId == courseId).ToListAsync(token);
                Context.CourseCategories.RemoveRange(relationToRemove);
                await Context.CourseCategories.AddRangeAsync
                    (categoryIds.Select(ci => CourseCategory.Create(courseId, ci)), token);
            }, token);
        }
    }
}
