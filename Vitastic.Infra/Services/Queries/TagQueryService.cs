using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Tags.Dtos;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

/// <summary>
/// Tag Query Service - Read operations only
/// Uses LINQ for simple queries and Dapper/Raw SQL for complex ones
/// </summary>
internal class TagQueryService(
    string connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    ILogger<TagQueryService> logger) : ITagQueryService
{
    // ==================== LIST (PAGINATION) ====================

    public async Task<(IReadOnlyList<TagDto> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool? onlyActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            IQueryable<Tag> query = readDbContext.Tags
            .AsNoTracking()
            .AsQueryable();

            if (onlyActive.HasValue)
                query = query.Where(t => t.IsActive == onlyActive.Value);

            var total = await query.CountAsync(cancellationToken);

            List<TagDto> items = await query
                .OrderByDescending(t => t.UsageCount)
                .ThenBy(t => t.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TagDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation(
                "Listed {Count} tags - Page {Page}/{Total}",
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPagedAsync");
            throw;
        }
    }

    // ==================== DETAIL ====================

    public async Task<TagDto?> GetDetailAsync(
        TagId tagId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Tag? tag = await readDbContext.Tags
                .FirstOrDefaultAsync(t => t.Id == tagId && t.IsActive, cancellationToken);

            if (tag is null)
            {
                logger.LogWarning("Tag not found: {TagId}", tagId);
                return null;
            }

            TagDto? detail = mapper.Map<TagDto>(tag);

            logger.LogInformation("Tag detail retrieved: {TagId}", tagId);

            return detail;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetDetailAsync");
            throw;
        }
    }

    // ==================== SEARCH ====================

    public async Task<(IReadOnlyList<TagDto> Items, int Total)> SearchAsync(
        string? searchTerm, int pageNumber, int pageSize, bool? onlyActive = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            logger.LogInformation(
                "Searching tags - Term: {SearchTerm}, OnlyActive: {OnlyActive}",
                searchTerm,
                onlyActive);


            IQueryable<Tag> query = readDbContext.Tags;

            if (onlyActive.HasValue)
                query = query.Where(t => t.IsActive==onlyActive.Value);
            var term = searchTerm?.ToLower().Trim();
            if (!string.IsNullOrEmpty(term))
            {
                var normalizedSearchTerm = $"%{term}%";

                query = readDbContext.Tags
                    .FromSqlRaw(""" SELECT * FROM "Tags" WHERE UPPER("Name") LIKE UPPER(@searchTerm) """
                        , new NpgsqlParameter("@searchTerm", normalizedSearchTerm));
            }
            var total = await query.CountAsync(cancellationToken);

            if (total == 0)
            {
                logger.LogInformation(
                    "No tags found for search term: {SearchTerm}",
                    searchTerm);
                return (new List<TagDto>(), 0);
            }

            var items = (await query
                .OrderByDescending(t => t.UsageCount)
                .ThenBy(t => t.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TagDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken))
                .ToList();

            logger.LogInformation(
                "Search completed - Term: {SearchTerm}, Results: {Count}, Page: {Page}/{Total}",
                searchTerm?.ToString(),
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

    // ==================== USAGE ====================

    public async Task<(IReadOnlyList<TagDto> Items, int Total)> GetMostUsedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            IOrderedQueryable<Tag> query = readDbContext.Tags
                .Where(t => t.IsActive && t.UsageCount > 0)
                .OrderByDescending(t => t.UsageCount)
                .ThenBy(t => t.Name.Value);

            var total = await query.CountAsync(cancellationToken);

            List<TagDto> items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TagDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation(
                "Retrieved {Count} most used tags - Page {Page}/{Total}",
                items.Count,
                pageNumber,
                (total + pageSize - 1) / pageSize);

            return (items, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetMostUsedAsync");
            throw;
        }
    }

    public async Task<IReadOnlyList<TagDto>> GetByUsageRangeAsync(
        int minUsage,
        int maxUsage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (minUsage < 0 || maxUsage < minUsage)
                throw new ArgumentException("Invalid usage range");

            List<TagDto> tags = await readDbContext.Tags
                .Where(t => t.UsageCount >= minUsage && t.UsageCount <= maxUsage && t.IsActive)
                .OrderByDescending(t => t.UsageCount)
                .ThenBy(t => t.Name.Value)
                .ProjectTo<TagDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation(
                "Retrieved {Count} tags in usage range {Min}-{Max}",
                tags.Count,
                minUsage,
                maxUsage);

            return tags;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByUsageRangeAsync");
            throw;
        }
    }

    // ==================== STATUS ====================

    public async Task<IReadOnlyList<TagDto>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tags = await readDbContext.Tags
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name.Value)
                .ProjectTo<TagDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Retrieved {Count} active tags", tags.Count);

            return tags;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetActiveAsync");
            throw;
        }
    }

    public async Task<IReadOnlyList<TagDto>> GetInactiveAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tags = await readDbContext.Tags
                .Where(t => !t.IsActive)
                .OrderBy(t => t.Name.Value)
                .ProjectTo<TagDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Retrieved {Count} inactive tags", tags.Count);

            return tags;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetInactiveAsync");
            throw;
        }
    }

    // ==================== STATISTICS ====================

    public async Task<TagStatisticsDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT
                    COUNT(*) as TotalTags,
                    COUNT(CASE WHEN IsActive = true THEN 1 END) as ActiveTags,
                    COUNT(CASE WHEN IsActive = false THEN 1 END) as InactiveTags,
                    COUNT(CASE WHEN UsageCount > 0 THEN 1 END) as UsedTags,
                    COALESCE(SUM(UsageCount), 0) as TotalUsage,
                    COALESCE(MAX(UsageCount), 0) as MaxUsage,
                    COALESCE(AVG(UsageCount), 0)::NUMERIC as AverageUsage
                FROM Tags";

            var result = await connection.QuerySingleAsync<TagStatisticsDto>(sql);

            logger.LogInformation("Tag statistics retrieved");

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetStatisticsAsync");
            throw;
        }
    }

}
