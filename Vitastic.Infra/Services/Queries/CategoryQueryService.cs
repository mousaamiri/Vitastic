using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

/// <summary>
///  We used LINQ for simple queries, but for more complex queries we can use Dapper or raw SQL.
/// </summary>
/// <param name="connectionString"> Connection string for connect to pg database </param>
/// <param name="readDbContext"> Use db context to simple queries </param>
/// <param name="logger"> Logger to log information </param>
internal class CategoryQueryService(
    string? connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    ILogger<CategoryQueryService> logger) : ICategoryQueryService
{
    // ==================== LIST (Pagination) ====================

    #region GetListedAsync - GET CATEGROIES TREE

    public async Task<List<CategoryDetailDto>> GetListedAsync(
        bool? onlyParents = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<Category> query = readDbContext.Categories
                .Where(c => c.IsActive)
                .AsNoTracking();

            if (onlyParents == true)
                query = query.Where(c => c.ParentCategoryId == null);

            List<CategoryDetailDto> items = await query
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ProjectTo<CategoryDetailDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Listed {Count} categories (OnlyParents: {OnlyParents})",
                items.Count, onlyParents ?? false);

            if (onlyParents != true)
            {
                var hierarchicalList = BuildHierarchy(items);
                PopulateParentNames(hierarchicalList);
                logger.LogInformation("Built hierarchical structure with {Count} root categories",
                    hierarchicalList.Count);
                return hierarchicalList;
            }

            return items;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing categories (OnlyParents: {OnlyParents})", onlyParents);
            throw;
        }
    }

    private List<CategoryDetailDto> BuildHierarchy(List<CategoryDetailDto> flatList)
    {
        var lookup = flatList.ToDictionary(c => c.Id);

        var rootCategories = new List<CategoryDetailDto>();

        foreach (var category in flatList)
        {
            if (category.ParentCategoryId == null)
            {
                rootCategories.Add(category);
            }
            else if (lookup.TryGetValue(category.ParentCategoryId.Value, out var parent))
            {
                parent.SubCategories ??= new List<CategoryDetailDto>();
                parent.SubCategories.Add(category);
            }
        }

        return rootCategories;
    }

    private void PopulateParentNames(List<CategoryDetailDto> categories, string? parentName = null)
    {
        foreach (CategoryDetailDto category in categories)
        {
            category.ParentCategoryName = parentName;

            if (category.SubCategories.Any()) PopulateParentNames(category.SubCategories, category.Name);
        }
    }

    #endregion


    public async Task<(IReadOnlyCollection<CategoryListDto> items, int count)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool? onlyParents = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                logger.LogWarning("Invalid pagination parameters - PageNumber: {PageNumber}, PageSize: {PageSize}",
                    pageNumber, pageSize);
                throw new ArgumentException("Invalid pagination parameters");
            }

            IQueryable<Category> query = readDbContext.Categories
                .Where(c => c.IsActive)
                .AsNoTracking();

            if (onlyParents == true)
                query = query.Where(c => c.ParentCategoryId == null);

            var count = await query.CountAsync(cancellationToken);

            List<CategoryListDto> items = await query
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CategoryListDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Listed {Count} categories from total {Total} (OnlyParents: {OnlyParents})",
                items.Count, count, onlyParents ?? false);

            return (items, count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing categories (OnlyParents: {OnlyParents})", onlyParents);
            throw;
        }
    }


    // ==================== DETAIL ====================

    public async Task<CategoryDetailDto?> GetDetailAsync(
        CategoryId categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get main categories
            Category? category = await readDbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);

            if (category is null)
            {
                logger.LogWarning("Category not found: {CategoryId}", categoryId);
                return null;
            }

            // Get parent category name
            string? parentName = null;
            if (category.ParentCategoryId != null)
            {
                parentName = await readDbContext.Categories
                    .Where(c => c.Id == category.ParentCategoryId)
                    .Select(c => c.Name.Value)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            // Get all categories
            var allCategories = await readDbContext.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(cancellationToken);

            var detail = mapper.Map<CategoryDetailDto>(category);
            detail.ParentCategoryName = parentName;
            detail.SubCategories = BuildCategoryTree(allCategories, category.Id);

            logger.LogInformation("Category detail with full tree retrieved: {CategoryId}", categoryId);

            return detail;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetDetailAsync");
            throw;
        }
    }

    private List<CategoryDetailDto> BuildCategoryTree(List<Category> allCategories, CategoryId categoryId)
    {
        var children = allCategories.Where(c => c.ParentCategoryId != null
                                                && c.ParentCategoryId.Equals(categoryId))
            .Select(c =>
            {
                var dto = mapper.Map<CategoryDetailDto>(c);
                dto.SubCategories = BuildCategoryTree(allCategories, c.Id);
                return dto;
            }).ToList();
        return children;
    }

    // ==================== SEARCH ====================
    public async Task<(IReadOnlyList<CategoryListDto> Items, int Total)> SearchAsync(
        string searchTerm, int pageNumber, int pageSize, bool onlyParents = false,
        bool onlyActive = true, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);

            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            logger.LogInformation(
                "Searching categories - Term: {SearchTerm}, Filters: OnlyParents={OnlyParents}, OnlyActive={OnlyActive}",
                searchTerm, onlyParents, onlyActive);

            //  Build query dynamically
            IQueryable<Category> query = readDbContext.Categories.AsQueryable();

            // Optional filters
            if (onlyActive)
                query = query.Where(c => c.IsActive);

            if (onlyParents)
                query = query.Where(c => c.ParentCategoryId == null);

            // Get total count
            var total = await query.CountAsync(cancellationToken);

            if (total == 0)
            {
                logger.LogInformation("No categories found for search term: {SearchTerm}", searchTerm);
                return (new List<CategoryListDto>(), 0);
            }

            var term = searchTerm.ToLower().Trim();
            // Get paginated items
            var items =
                (await query
                    .OrderBy(c => c.DisplayOrder)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ProjectTo<CategoryListDto>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken))
                .Where(x => x.Name.ToLower().Contains(term))
                .ToList();

            logger.LogInformation(
                "Search completed successfully - SearchTerm: {SearchTerm}, Results: {Count}, Page: {Page}/{Total}",
                searchTerm, items.Count, pageNumber, (total + pageSize - 1) / pageSize);

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

    // ==================== HIERARCHY (DAPPER) ====================

    public async Task<IReadOnlyList<CategoryTreeDto>> GetTreeAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            const string sql = @"
                WITH CategoryTree AS (
                    SELECT
                        CategoryId,
                        Name,
                        Slug,
                        Description,
                        DisplayOrder,
                        CAST(NULL AS UNIQUEIDENTIFIER) as ParentId,
                        0 as Level
                    FROM Categories
                    WHERE ParentCategoryId IS NULL AND IsActive = 1

                    UNION ALL

                    SELECT
                        c.CategoryId,
                        c.Name,
                        c.Slug,
                        c.ParentCategoryId,
                        c.DisplayOrder,
                        ct.Level + 1
                    FROM Categories c
                    INNER JOIN CategoryTree ct ON c.ParentCategoryId = ct.CategoryId
                    WHERE c.IsActive = 1
                )
                SELECT
                    CategoryId as Id,
                    Name,
                    Slug,
                    ParentId,
                    DisplayOrder,
                    Level
                FROM CategoryTree
                ORDER BY Level, DisplayOrder";

            IEnumerable<CategoryTreeDto> result = (await conn.QueryAsync<CategoryTreeDto>(sql)).ToList();

            logger.LogInformation("Category tree retrieved with {Count} items", result.Count());

            return result.ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetTreeAsync");
            throw;
        }
    }

    // ==================== CHILDREN ====================

    public async Task<IReadOnlyList<SubCategoryDto>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            List<SubCategoryDto> children = await readDbContext.Categories
                .Where(c => c.ParentCategoryId.Value == parentId && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ProjectTo<SubCategoryDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogInformation(
                "Retrieved {Count} children for parent {ParentId}",
                children.Count,
                parentId);

            return children;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetChildrenAsync");
            throw;
        }
    }

    // ==================== STATISTICS ====================

    public async Task<CategoryStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT
                    COUNT(*) as TotalCategories,
                    COUNT(CASE WHEN IsActive = 1 THEN 1 END) as ActiveCategories,
                    COUNT(CASE WHEN ParentCategoryId IS NOT NULL THEN 1 END) as SubCategoriesCount,
                    COUNT(DISTINCT ParentCategoryId) as ParentCategoriesCount
                FROM Categories";

            CategoryStatisticsDto result = await connection.QuerySingleAsync<CategoryStatisticsDto>(sql);

            logger.LogInformation("Category statistics retrieved");

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetStatisticsAsync");
            throw;
        }
    }
}
