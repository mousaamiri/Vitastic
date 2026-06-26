using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.Enums;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

/// <summary>
/// Instructor Query Service - Read operations only
/// Uses LINQ for simple queries and Dapper/Raw SQL for complex ones
/// </summary>
internal class InstructorQueryService(
    string? connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    ILogger<InstructorQueryService> logger) : IInstructorQueryService
{
    // ==================== LIST (PAGINATION) ====================

    public async Task<(IReadOnlyList<InstructorDto> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        InstructorStatusDto? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            IQueryable<Instructor> query = readDbContext.Instructors
                .Include(i => i.Ratings)
                .Where(i => i.Status.Equals(InstructorStatus.Active))
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(i => (int)i.Status == (int)status.Value);

            var total = await query.CountAsync(cancellationToken);

            List<Instructor> items = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            var dtos = mapper.Map<List<InstructorDto>>(items);
            logger.LogInformation(
                "Listed {Count} instructors - Page {Page}/{Total}, Status: {Status}",
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize,
                status?.ToString() ?? "All");

            return (dtos, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPagedAsync");
            throw;
        }
    }

    // ==================== DETAIL ====================

    public async Task<InstructorDetailDto?> GetByIdAsync(
        InstructorId instructorId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var instructor = await readDbContext.Instructors
                .Include(i => i.Ratings)
                .FirstOrDefaultAsync(i => i.Id.Equals(instructorId), cancellationToken);

            if (instructor is null)
            {
                logger.LogWarning("Instructor not found: {InstructorId}", instructorId);
                return null;
            }

            InstructorDetailDto? detail = mapper.Map<InstructorDetailDto>(instructor);
            //detail.Skills = instructor.Skills?.Values.Select(s => s.Value).ToList() ?? [];

            logger.LogInformation("Instructor detail retrieved: {InstructorId}", instructorId);

            return detail;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByIdAsync");
            throw;
        }
    }

    public async Task<InstructorDetailDto?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var instructor = await readDbContext.Instructors
                .Include(i => i.Ratings)
                .FirstOrDefaultAsync(i => i.UserId.Value == userId, cancellationToken);

            if (instructor is null)
            {
                logger.LogWarning("Instructor not found for user: {UserId}", userId);
                return null;
            }

            InstructorDetailDto? detail = mapper.Map<InstructorDetailDto>(instructor);
            //detail.Skills = instructor.Skills?.Values.Select(s => s.Value).ToList() ?? [];

            logger.LogInformation("Instructor detail retrieved for user: {UserId}", userId);

            return detail;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByUserIdAsync");
            throw;
        }
    }

    // ==================== SEARCH ====================

    public async Task<(IReadOnlyList<InstructorDto> Items, int Total)> SearchAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        InstructorStatusDto? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);

            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            logger.LogInformation(
                "Searching instructors - Term: {SearchTerm}, Status: {Status}",
                searchTerm,
                status?.ToString() ?? "All");


            IQueryable<Instructor> query = readDbContext.Instructors
                .Include(i => i.Ratings);

            if (status.HasValue)
            {
                var state = (InstructorStatus)status.Value;
                query = query.Where(i => i.Status.Equals(state));
            }

            var total = await query.CountAsync(cancellationToken);

            if (total == 0)
            {
                logger.LogInformation(
                    "No instructors found for search term: {SearchTerm}",
                    searchTerm);
                return (new List<InstructorDto>(), 0);
            }

            var term = searchTerm.Trim().ToLower();
            var items = (await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ProjectTo<InstructorDto>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken))
                .Where(i =>
                    i.FullName.ToLower().Contains(term) || i.Bio.ToLower().Contains(term))
                //i.User.Email.ToLower().Contains(term))) // we use dto in here
                .ToList();

            logger.LogInformation(
                "Search completed - Term: {SearchTerm}, Results: {Count}, Page: {Page}/{Total}",
                searchTerm,
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Search operation cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SearchAsync");
            throw;
        }
    }

    // ==================== STATUS ====================

    public async Task<(IReadOnlyList<InstructorDto> Items, int Total)> GetByStatusAsync(
        InstructorStatus status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            var query = readDbContext.Instructors
                .Include(i => i.Ratings)
                .Where(i => i.Status == status)
                .OrderByDescending(i => i.CreatedAt);

            var total = await query.CountAsync(cancellationToken);

            List<InstructorDto> items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<InstructorDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation(
                "Retrieved {Count} instructors with status {Status} - Page {Page}/{Total}",
                items.Count,
                status,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByStatusAsync");
            throw;
        }
    }

    // ==================== SKILLS ====================

    public async Task<IReadOnlyList<InstructorDto>> GetBySkillAsync(
        string skill,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(skill);

            var skillLower = skill.Trim().ToLower();

            List<InstructorDto> instructors = await readDbContext.Instructors
                .Include(i => i.Ratings)
                .Where(i =>
                    i.Status == InstructorStatus.Active &&
                    i.Skills.Values.Any(s => s.Value.ToLower().Contains(skillLower)))
                .OrderBy(i => i.FullName)
                .ProjectTo<InstructorDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation(
                "Retrieved {Count} instructors with skill: {Skill}",
                instructors.Count,
                skill);

            return instructors;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetBySkillAsync");
            throw;
        }
    }

    public async Task<IReadOnlyList<string>> GetSkillsAsync(
        Guid instructorId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Instructor? instructor = await readDbContext.Instructors
                .Include(i => i.Ratings)
                .FirstOrDefaultAsync(i => i.Id.Value == instructorId, cancellationToken);

            if (instructor is null)
            {
                logger.LogWarning("Instructor not found for skills: {InstructorId}", instructorId);
                return new List<string>();
            }

            var skills = instructor.Skills.Values
                .Select(s => s.Value)
                .ToList();

            logger.LogInformation(
                "Retrieved {Count} skills for instructor: {InstructorId}",
                skills.Count,
                instructorId);

            return skills;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetSkillsAsync");
            throw;
        }
    }

    // ==================== STATISTICS ====================

    public async Task<InstructorStatisticsDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT
                    COUNT(*) as TotalInstructors,
                    COUNT(CASE WHEN Status = 1 THEN 1 END) as ActiveInstructors,
                    COUNT(CASE WHEN Status = 0 THEN 1 END) as InactiveInstructors,
                    COUNT(CASE WHEN Status = 2 THEN 1 END) as PendingInstructors
                FROM Instructors";

            var result = await connection.QuerySingleAsync<InstructorStatisticsDto>(sql);

            logger.LogInformation("Instructor statistics retrieved");

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetStatisticsAsync");
            throw;
        }
    }

    // ==================== UTILITIES ====================

    public async Task<bool> IsInstructorNameExistAsync(
        FullName fullName,
        Guid? excludeInstructorId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

            var query = readDbContext.Instructors
                .Where(i => i.FullName == fullName);

            if (excludeInstructorId.HasValue)
                query = query.Where(i => i.Id.Value != excludeInstructorId.Value);

            var exists = await query.AnyAsync(cancellationToken);

            logger.LogInformation(
                "Instructor name existence check: {FullName}, Exists: {Exists}",
                fullName,
                exists);

            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in IsInstructorNameExistAsync");
            throw;
        }
    }

    public async Task<bool> IsUserInstructorAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await readDbContext.Instructors
                .AnyAsync(i => i.UserId.Value == userId, cancellationToken);

            logger.LogInformation(
                "User instructor check: {UserId}, IsInstructor: {IsInstructor}",
                userId,
                exists);

            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in IsUserInstructorAsync");
            throw;
        }
    }

    public async Task<int> GetActiveInstructorsCountAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await readDbContext.Instructors
                .Where(i => i.Status == InstructorStatus.Active)
                .CountAsync(cancellationToken);

            logger.LogInformation("Active instructors count: {Count}", count);

            return count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetActiveInstructorsCountAsync");
            throw;
        }
    }
}
