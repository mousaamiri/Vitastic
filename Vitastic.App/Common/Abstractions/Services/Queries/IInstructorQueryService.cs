using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.Domain.Entities.Instructors.Enums;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

/// <summary>
/// Query Service Interface for Instructor Entity
/// For READ operations only
/// </summary>
public interface IInstructorQueryService
{
    // ==================== LIST ====================
    Task<(IReadOnlyList<InstructorDto> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        InstructorStatusDto? status = null,
        CancellationToken cancellationToken = default);

    // ==================== DETAIL ====================
    Task<InstructorDetailDto?> GetByIdAsync(
        InstructorId instructorId,
        CancellationToken cancellationToken = default);

    Task<InstructorDetailDto?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    // ==================== SEARCH ====================
    Task<(IReadOnlyList<InstructorDto> Items, int Total)> SearchAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        InstructorStatusDto? status = null,
        CancellationToken cancellationToken = default);

    // ==================== STATUS ====================
    Task<(IReadOnlyList<InstructorDto> Items, int Total)> GetByStatusAsync(
        InstructorStatus status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    // ==================== SKILLS ====================
    Task<IReadOnlyList<InstructorDto>> GetBySkillAsync(
        string skill,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetSkillsAsync(
        Guid instructorId,
        CancellationToken cancellationToken = default);

    // ==================== STATISTICS ====================
    Task<InstructorStatisticsDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default);

    // ==================== UTILITIES ====================
    Task<bool> IsInstructorNameExistAsync(
        FullName fullName,
        Guid? excludeInstructorId = null,
        CancellationToken cancellationToken = default);

    Task<bool> IsUserInstructorAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<int> GetActiveInstructorsCountAsync(
        CancellationToken cancellationToken = default);
}
