using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface IRolePermissionRepository : IRepository<RolePermission, RolePermissionId>
{
    Task<bool> RelationIsExist(RolePermission relation, CancellationToken token=default);
    Task<RolePermission?> FindAsync(RoleId roleId,PermissionId permissionId, CancellationToken token=default);
}
public interface ICourseRatingRepository : IRepository<CourseRating,CourseRatingId>
{
    Task<bool> RelationIsExist(CourseRating relation, CancellationToken token=default);
    Task<CourseRating?> FindAsync(CourseId courseId,UserId userId, CancellationToken token=default);
}
public interface IInstructorRatingRepository : IRepository<InstructorRating, InstructorRatingId>
{
    Task<bool> RelationIsExist(InstructorRating relation, CancellationToken token=default);
    Task<InstructorRating?> FindAsync(InstructorId instructorId,UserId userId, CancellationToken token=default);
}
