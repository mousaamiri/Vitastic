using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Infra.Repositories;

internal class RolePermissionRepository(ApplicationWriteDbContext dbContext)
    : BaseRepository<RolePermission, RolePermissionId>(dbContext), IRolePermissionRepository
{
    public async Task<bool> RelationIsExist(RolePermission relation, CancellationToken token = default)
        => await ExecuteAsync(
            async () => await
                Context.RolePermissions
                    .AnyAsync(r =>
                            (r.PermissionId == relation.PermissionId && r.RoleId == relation.RoleId)
                            || (r.Id == relation.Id)
                        , token), token);

    public async Task<RolePermission?> FindAsync(RoleId roleId, PermissionId permissionId, CancellationToken token = default)
        => await ExecuteAsync(
            async () => await
                Context.RolePermissions
                    .FirstOrDefaultAsync(r => r.PermissionId == permissionId && r.RoleId == roleId
                        , token), token);
}

internal class InstructorRatingRepository(ApplicationWriteDbContext dbContext)
    : BaseRepository<InstructorRating, InstructorRatingId>(dbContext), IInstructorRatingRepository
{
    public async Task<bool> RelationIsExist(InstructorRating relation, CancellationToken token = default)
        => await ExecuteAsync(
            async () => await
                Context.InstructorRatings
                    .AnyAsync(r =>
                            (r.InstructorId == relation.InstructorId && r.UserId == relation.UserId)
                            || (r.Id == relation.Id)
                        , token), token);

    public async Task<InstructorRating?> FindAsync(InstructorId instructorId, UserId userId, CancellationToken token = default)
        => await ExecuteAsync(
            async () => await
                Context.InstructorRatings
                    .FirstOrDefaultAsync(r => r.InstructorId == instructorId && r.UserId == userId
                        , token), token);
}

internal class CourseRatingRepository(ApplicationWriteDbContext dbContext)
    : BaseRepository<CourseRating, CourseRatingId>(dbContext), ICourseRatingRepository
{
    public async Task<bool> RelationIsExist(CourseRating relation, CancellationToken token = default)
        => await ExecuteAsync(
            async () => await
                Context.CourseRatings
                    .AnyAsync(r =>
                            (r.CourseId == relation.CourseId && r.UserId == relation.UserId)
                            || (r.Id == relation.Id)
                        , token), token);

    public async Task<CourseRating?> FindAsync(CourseId courseId, UserId userId, CancellationToken token = default)
        => await ExecuteAsync(
            async () => await
                Context.CourseRatings
                    .FirstOrDefaultAsync(r => r.CourseId == courseId && r.UserId == userId
                        , token), token);
}
