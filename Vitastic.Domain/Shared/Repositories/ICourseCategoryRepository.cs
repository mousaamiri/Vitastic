using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories
{
    public interface ICourseCategoryRepository : IRepository<CourseCategory, CourseCategoryId>
    {
        Task<bool> RelationIsExist(CourseCategory relation, CancellationToken token=default);
        Task<bool> RelationIsExist(CourseId courseId,CategoryId categoryId, CancellationToken token=default);
        Task<CourseCategory?> FindAsync(CourseId courseId,CategoryId categoryId, CancellationToken token=default);
        Task ReassignCourseCategoriesAsync(CourseId courseId,IEnumerable<CategoryId> categoryIds,  CancellationToken token=default);
    }
}
