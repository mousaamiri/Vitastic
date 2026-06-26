using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

internal class CourseQueryService(
    string? connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    IFileUrlService fileUrlService,
    ILogger<CourseQueryService> logger,
    IFileUrlService urlService) : ICourseQueryService
{
    #region Get Single Course Methods

    public async Task<CourseDto?> GetBySlugAsync(Slug slug, UserId? userId, string? sessionId,
        CancellationToken token = default)
    {
        try
        {
            Course? course = await readDbContext.Courses
                .Include(c => c.Sections)
                .ThenInclude(s => s.Episodes)
                .FirstOrDefaultAsync(c => c.Slug == slug, token);

            await AssignTagAndCourses(course, token);
            return await MapCourseToCourseDto(userId, sessionId, course);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetBySlugAsync");
            throw;
        }
    }

    public async Task<CourseDto?> GetWithSectionsByCourseIdAsync(CourseId id, UserId? userId,
        string? sessionId,
        CancellationToken token = default)
    {
        try
        {
            Course? course = await readDbContext.Courses
                .Include(c => c.Sections)
                .ThenInclude(s => s.Episodes)
                .FirstOrDefaultAsync(c => c.Id == id, token);

            await AssignTagAndCourses(course, token);
            return await MapCourseToCourseDto(userId, sessionId, course);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetWithSectionsByCourseIdAsync");
            throw;
        }
    }

    public async Task<CourseDto?> GetByIdAsync(CourseId value, UserId? userId, CancellationToken token = default)
    {
        try
        {
            Course? course = await readDbContext.Courses
                .FirstOrDefaultAsync(c => c.Id == value, token);

            await AssignTagAndCourses(course, token);
            return await MapCourseToCourseDto(userId, null, course);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByIdAsync");
            throw;
        }
    }

    #endregion

    #region Get Paged Courses Methods

    public async Task<(IReadOnlyList<SimpleCourseDto> courses, int totalCount)>
        GetInstructorCoursesAsync(InstructorId id,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            IQueryable<Course> query = readDbContext.Courses
                .AsNoTracking()
                .Where(c => c.InstructorId == id);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var dtos = mapper.Map<List<SimpleCourseDto>>(items);
            return (dtos, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetInstructorCoursesAsync");
            throw;
        }
    }

    public async Task<(IReadOnlyList<SimpleCourseDto> courses, int totalCount)> GetPagedAsync(int pageNumber,
        int pageSize, UserId? userId, string? sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            IQueryable<Course> query = readDbContext.Courses
                .Include(c => c.Ratings)
                .Include(c => c.Sections)
                .ThenInclude(s => s.Episodes)
                .Include(s => s.Ratings)
                .AsNoTracking();

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            List<SimpleCourseDto> dtos = mapper.Map<List<SimpleCourseDto>>(items);
            await AssignCoursesInstructors(dtos);

            // Set purchase and cart status
            await SetPurchaseAndCartStatus(dtos, userId, sessionId);

            return (dtos, total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPagedAsync");
            throw;
        }
    }

    #endregion

    #region Search Courses
public async Task<(IReadOnlyList<SimpleCourseDto> courses, int totalCount)> SearchAsync(
    string searchTerm, int pageNumber, int pageSize,
    Guid? instructorId, Guid? categoryId,
    CourseLevel? level, CourseStatus? status,
    DateTimeOffset? fromDate, DateTimeOffset? toDate,
    CourseSortBy? sortBy,
    decimal? minPrice, decimal? maxPrice,
    bool? hasCertificate, bool? isFree,
    UserId? userId, string? sessionId,
    CancellationToken cancellationToken = default)
{
    try
    {
        #region Validation
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            throw new ArgumentException("پارامترهای صفحه‌بندی نامعتبر است");
        #endregion

        #region Base Query
        IQueryable<Course> query = readDbContext.Courses
            .Include(c => c.Ratings)
            .Include(c => c.Sections).ThenInclude(s => s.Episodes)
            .AsNoTracking();
        #endregion

        #region Simple EF Filters
        if (instructorId.HasValue)
            query = query.Where(c => c.InstructorId.Value.Equals(instructorId.Value));

        if (categoryId.HasValue)
        {
            var courseIdsInCategory = readDbContext.CourseCategories
                .Where(cc => cc.CategoryId == categoryId.Value)
                .Select(cc => cc.CourseId);
            query = query.Where(c => courseIdsInCategory.Contains(c.Id));
        }

        if (level.HasValue)
            query = query.Where(c => c.Level == level.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(c => c.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(c => c.CreatedAt <= toDate.Value);

        if (hasCertificate.HasValue)
            query = query.Where(c => c.HasCertificate.Equals(hasCertificate.Value));
        #endregion

        #region Search & Price Filter via Raw SQL
        // Build a raw SQL query to get IDs matching search + price filters
        // because EF Core cannot translate Value Object properties to SQL
        var sqlBuilder = new StringBuilder("""
            SELECT "Id" FROM "Courses" WHERE "IsDeleted" = false
            """);
        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim().ToLower()}%";
            sqlBuilder.Append("""
                 AND (LOWER("Title") LIKE @term OR LOWER("ShortDescription") LIKE @term)
                """);
            parameters.Add(new NpgsqlParameter("term", term));
        }

        // Price subquery: sum of all episode prices per course
        const string priceSubQuery = """
            (SELECT COALESCE(SUM(CAST(e."Price" AS DECIMAL)), 0)
             FROM "Sections" s JOIN "Episodes" e ON e."SectionId" = s."Id"
             WHERE s."CourseId" = "Courses"."Id"AND s."IsDeleted" = false AND e."IsDeleted" = false)
            """;

        if (isFree == true)
            sqlBuilder.Append($" AND {priceSubQuery} = 0");
        else
        {
            if (minPrice.HasValue)
            {
                sqlBuilder.Append($" AND {priceSubQuery} >= @minPrice");
                parameters.Add(new NpgsqlParameter("minPrice", minPrice.Value));
            }
            if (maxPrice.HasValue)
            {
                sqlBuilder.Append($" AND {priceSubQuery} <= @maxPrice");
                parameters.Add(new NpgsqlParameter("maxPrice", maxPrice.Value));
            }
        }

        // Only run raw SQL if any of these filters are active
        bool hasRawFilter = !string.IsNullOrWhiteSpace(searchTerm)|| isFree.HasValue || minPrice.HasValue || maxPrice.HasValue;

        if (hasRawFilter)
        {
            var filteredIds = await readDbContext.Database
                .SqlQueryRaw<Guid>(sqlBuilder.ToString(), parameters.ToArray())
                .ToListAsync(cancellationToken);

            query = query.Where(c => filteredIds.Contains(c.Id));
        }
        #endregion

        #region Count & Sort & Paginate
        // Count AFTER all filters are applied
        var total = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, sortBy);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        #endregion

        #region Map & Enrich
        var dtos = mapper.Map<List<SimpleCourseDto>>(items);
        await AssignCoursesInstructors(dtos);
        await SetPurchaseAndCartStatus(dtos, userId, sessionId);
        #endregion

        return (dtos, total);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "خطا در SearchAsync");
        throw;
    }
}

    #endregion
 #region Get My Enrolled Courses (Purchased Only)
public async Task<(IReadOnlyList<SimpleCourseDto> courses, int totalCount)> GetMyCoursesAsync(
    string searchTerm,
    int pageNumber,
    int pageSize,
    Guid? instructorId,
    Guid? categoryId,
    CourseLevel? level,
    CourseStatus? status,
    DateTimeOffset? fromDate,
    DateTimeOffset? toDate,
    CourseSortBy? sortBy,
    decimal? minPrice,
    decimal? maxPrice,
    bool? hasCertificate,
    bool? isFree,
    UserId userId,
    CancellationToken cancellationToken = default)
{
    try
    {
        #region Validation
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            throw new ArgumentException("پارامترهای صفحه‌بندی نامعتبر است.");
        #endregion

        var purchasedCourseIds = await GetUserPurchasedCourseIds(userId);
        if (!purchasedCourseIds.Any())
        {
            return (new List<SimpleCourseDto>(), 0);
        }

        #region Base Query - فقط دوره‌های خریداری‌شده
        IQueryable<Course> query = readDbContext.Courses
            .Where(c => purchasedCourseIds.Contains(c.Id))
            .Include(c => c.Ratings)
            .Include(c => c.Sections).ThenInclude(s => s.Episodes)
            .AsNoTracking();
        #endregion

        #region Simple EF Filters
        if (instructorId.HasValue)
            query = query.Where(c => c.InstructorId.Value.Equals(instructorId.Value));

        if (categoryId.HasValue)
        {
            var courseIdsInCategory = readDbContext.CourseCategories
                .Where(cc => cc.CategoryId == categoryId.Value)
                .Select(cc => cc.CourseId);
            query = query.Where(c => courseIdsInCategory.Contains(c.Id));
        }

        if (level.HasValue)
            query = query.Where(c => c.Level == level.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(c => c.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(c => c.CreatedAt <= toDate.Value);

        if (hasCertificate.HasValue)
            query = query.Where(c => c.HasCertificate == hasCertificate.Value);
        #endregion

        #region Search & Price Filter via Raw SQL (روی دوره‌های خریداری‌شده)
        var sqlBuilder = new StringBuilder("""
            SELECT "Id" FROM "Courses"
            WHERE "IsDeleted" = false
              AND "Id" = ANY(@purchasedCourseIds::uuid[])
            """);
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("purchasedCourseIds", purchasedCourseIds.ToArray())
        };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim().ToLower()}%";
            sqlBuilder.Append("""
                 AND (LOWER("Title") LIKE @term OR LOWER("ShortDescription") LIKE @term)
                """);
            parameters.Add(new NpgsqlParameter("term", term));
        }

        // Subquery for total course price (only active sections/episodes)
        const string priceSubQuery = """
            (SELECT COALESCE(SUM(CAST(e."Price" AS DECIMAL)), 0)
             FROM "Sections" s
             JOIN "Episodes" e ON e."SectionId" = s."Id"
             WHERE s."CourseId" = "Courses"."Id"
               AND s."IsDeleted" = false
               AND e."IsDeleted" = false)
            """;

        if (isFree == true)
        {
            sqlBuilder.Append($" AND {priceSubQuery} = 0");
        }
        else
        {
            if (minPrice.HasValue)
            {
                sqlBuilder.Append($" AND {priceSubQuery} >= @minPrice");
                parameters.Add(new NpgsqlParameter("minPrice", minPrice.Value));
            }
            if (maxPrice.HasValue)
            {
                sqlBuilder.Append($" AND {priceSubQuery} <= @maxPrice");
                parameters.Add(new NpgsqlParameter("maxPrice", maxPrice.Value));
            }
        }

        bool hasRawFilter = !string.IsNullOrWhiteSpace(searchTerm) || isFree.HasValue || minPrice.HasValue || maxPrice.HasValue;

        if (hasRawFilter)
        {
            var filteredIds = await readDbContext.Database
                .SqlQueryRaw<Guid>(sqlBuilder.ToString(), parameters.ToArray())
                .ToListAsync(cancellationToken);

            query = query.Where(c => filteredIds.Contains(c.Id));
        }
        #endregion

        #region Count, Sort & Paginate
        var total = await query.CountAsync(cancellationToken);
        query = ApplySorting(query, sortBy);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        #endregion

        #region Map & Enrich
        var dtos = mapper.Map<List<SimpleCourseDto>>(items);
        await AssignCoursesInstructors(dtos);

        // تمام دوره‌ها خریداری‌شده‌اند → IsPurchased = true
        foreach (var dto in dtos)
        {
            dto.IsPurchased = true;
            dto.IsInCart = false; // اختیاری: معمولاً دوره‌های خریداری‌شده در سبد نیستند
        }
        #endregion

        return (dtos, total);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "خطا در GetMyCoursesAsync");
        throw;
    }
}

#endregion

    #region Helper Methods - Mapping & Assignment

    private async Task AssignTagAndCourses(Course? course, CancellationToken token = default)
    {
        if (course is not null)
        {
            course.SetTags(await readDbContext.CourseTags
                .Where(t => t.CourseId == course.Id)
                .AsNoTracking()
                .Select(x => x.TagId)
                .ToListAsync(token));

            course.SetCategories(await readDbContext.CourseCategories
                .Where(t => t.CourseId == course.Id)
                .AsNoTracking()
                .Select(x => x.CategoryId)
                .ToListAsync(token));
        }
    }

    private async Task<CourseDto?> MapCourseToCourseDto(UserId? userId, string? sessionId, Course? course)
    {
        CourseDto dto = mapper.Map<CourseDto>(course);
        await SetPurchaseAndCartStatus(dto, userId, sessionId);
        return dto;
    }

    private async Task AssignCoursesInstructors(List<SimpleCourseDto> items)
    {
        var instructorIds = items.Select(c => c.InstructorId).Distinct().ToList();

        List<Instructor> instructors = await readDbContext.Instructors
            .Where(i => instructorIds.Contains(i.Id))
            .Include(i => i.Ratings)
            .ToListAsync();

        foreach (SimpleCourseDto courseDto in items)
        {
            Instructor? instructor = instructors.FirstOrDefault(i => i.Id == courseDto.InstructorId);
            if (instructor == null) continue;

            courseDto.Instructor = new CourseInstructorDto
            {
                Id = instructor.Id,
                Name = instructor.FullName,
                Avatar = urlService.GetFileUrl(nameof(Instructor), instructor.Id.Value, FileType.Image,
                    instructor.Avatar),
                AverageRating = instructor.AverageRating,
                TotalRatings = instructor.TotalRatings,
                Experties = instructor.Expertise.Value
            };
        }
    }

    #endregion

    #region Purchase & Cart Status Methods

    /// <summary>
    /// Sets IsPurchased and IsInCart for a list of courses.
    /// Handles both authenticated users (userId) and guests (sessionId).
    /// </summary>
    private async Task SetPurchaseAndCartStatus(List<SimpleCourseDto> dtos, UserId? userId, string? sessionId)
    {
        // Check purchased courses (only for authenticated users)
        if (userId != null)
        {
            List<Guid> purchasedCourseIds = await GetUserPurchasedCourseIds(userId);
            foreach (SimpleCourseDto courseDto in dtos)
                courseDto.IsPurchased = purchasedCourseIds.Contains(courseDto.Id);
        }

        // Check cart status (for both authenticated and guest users)
        List<Guid> cartCourseIds = await GetCartCourseIds(userId, sessionId);
        foreach (SimpleCourseDto courseDto in dtos)
            courseDto.IsInCart = cartCourseIds.Contains(courseDto.Id);
    }

    /// <summary>
    /// Sets IsPurchased and IsInCart for a single course.
    /// Handles both authenticated users (userId) and guests (sessionId).
    /// </summary>
    private async Task SetPurchaseAndCartStatus(CourseDto courseDto, UserId? userId, string? sessionId)
    {
        // Check purchased courses (only for authenticated users)
        if (userId != null)
        {
            List<Guid> purchasedCourseIds = await GetUserPurchasedCourseIds(userId);
            courseDto.IsPurchased = purchasedCourseIds.Contains(courseDto.Id);
        }

        // Check cart status (for both authenticated and guest users)
        List<Guid> cartCourseIds = await GetCartCourseIds(userId, sessionId);
        courseDto.IsInCart = cartCourseIds.Contains(courseDto.Id);
    }

    /// <summary>
    /// Gets all course IDs in the user's or guest's cart.
    /// </summary>
    private async Task<List<Guid>> GetCartCourseIds(UserId? userId, string? sessionId)
    {
        IQueryable<Guid> query;

        if (userId != null)
        {
            // Authenticated user cart
            query = readDbContext.Carts
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.Items)
                .Select(ci => ci.CourseId.Value);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            // Guest cart
            query = readDbContext.Carts
                .Where(c => c.SessionId == sessionId)
                .SelectMany(c => c.Items)
                .Select(ci => ci.CourseId.Value);
        }
        else
        {
            // No user or session - return empty
            return [];
        }

        return await query.Distinct().ToListAsync();
    }

    /// <summary>
    /// Gets all purchased course IDs for an authenticated user.
    /// </summary>
    private async Task<List<Guid>> GetUserPurchasedCourseIds(UserId userId)
    {
        return await readDbContext.Orders
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Completed)
            .SelectMany(o => o.Items)
            .Select(ci => ci.CourseId.Value)
            .Distinct()
            .ToListAsync();
    }

    #endregion

    #region Sorting

    /// <summary>
    /// Applies sorting to course query based on the given sort type.
    /// Uses Orders (Invoices) table for BestSelling calculation.
    /// </summary>
    private IQueryable<Course> ApplySorting(IQueryable<Course> query, CourseSortBy? sortBy)
    {
        return sortBy switch
        {
            CourseSortBy.Oldest => query.OrderBy(c => c.CreatedAt),

            CourseSortBy.PriceAscending => query.OrderBy(c => c.Price.Value),

            CourseSortBy.PriceDescending => query.OrderByDescending(c => c.Price.Value),

            CourseSortBy.HighestRated => query.OrderByDescending(c =>
                c.Ratings.Any() ? c.AverageRating : 0),

            CourseSortBy.BestSelling => query.OrderByDescending(c =>
                readDbContext.Orders
                    .Where(o => o.Status == OrderStatus.Completed)
                    .SelectMany(o => o.Items)
                    .Count(oi => oi.CourseId == c.Id)),

            CourseSortBy.MostPopular => query.OrderByDescending(c =>
                    readDbContext.Orders
                        .Where(o => o.Status == OrderStatus.Completed)
                        .SelectMany(o => o.Items)
                        .Count(oi => oi.CourseId == c.Id))
                .ThenByDescending(c => c.Ratings.Count),

            _ => query.OrderByDescending(c => c.CreatedAt)
        };
    }

    #endregion
}
