using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal sealed class InstructorRepository(ApplicationWriteDbContext dbContext)
    : BaseRepository<Instructor, InstructorId>(dbContext), IInstructorRepository
{
    public async Task<bool> ExistsAsync(InstructorId instructorId, CancellationToken token = default)
        => await ExecuteAsync(async ()
                => await Context.Instructors.AnyAsync(s => s.Id == instructorId, token), token);

    public async Task<List<Instructor>> GetByIdsAsync(List<CourseId> courseIds, CancellationToken token = default)
    {
        return await ExecuteAsync(async () =>
            {
                var courseInstructorIds =
                    await Context.Courses.Where(c => courseIds.Contains(c.Id))
                    .Select(c => c.InstructorId)
                    .ToListAsync(token);
                var instructors =await Context.Instructors
                    .Where(i => courseInstructorIds.Contains(i.Id))
                    .ToListAsync(token);
                return instructors;
            }
            , token);
    }

    public async Task<FullName?> GetCourseInstructorFullName(InstructorId courseInstructorId,
        CancellationToken token = default)
        => await ExecuteAsync(async () =>
            await Context.Instructors
                .Where(i=>i.Id == courseInstructorId)
                .Select(instructor => instructor.FullName)
                .FirstAsync(token)
                , token);
}
