using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories
{
    public interface ICourseTagRepository : IRepository<CourseTag, CourseTagId>
    {
        Task<bool> RelationIsExist(CourseTag relation, CancellationToken token=default);
        Task<bool> RelationIsExist(CourseId courseId,TagId tagId, CancellationToken token=default);
        Task<CourseTag?> FindAsync(CourseId courseId,TagId tagId, CancellationToken token=default);
        Task ReassignCourseTagsAsync(CourseId courseId, IEnumerable<TagId> tagIds, CancellationToken token=default);
    }
}
