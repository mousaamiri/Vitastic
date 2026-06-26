using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface IInstructorRepository : IRepository<Instructor, InstructorId>
{
    Task<bool> ExistsAsync(InstructorId instructorId, CancellationToken token=default);
    Task<List<Instructor>> GetByIdsAsync(List<CourseId> courseIds, CancellationToken token=default);
    Task<FullName?> GetCourseInstructorFullName(InstructorId courseInstructorId, CancellationToken token=default);
}
