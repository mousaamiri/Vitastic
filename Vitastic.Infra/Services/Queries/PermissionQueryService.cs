using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

internal class PermissionQueryService(
    string? connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    ILogger<PermissionQueryService> logger) : IPermissionQueryService
{
    public async Task<RolePermissionDto?> GetByIdAsync(PermissionId permissionId, CancellationToken token = default)
    {
        try
        {
            Permission? permission = await readDbContext.Permissions
                .FirstOrDefaultAsync(t => t.Id == permissionId, token);

            if (permission is null)
            {
                logger.LogWarning("Permission not found: {PermissionId}", permissionId);
                return null;
            }
            RolePermissionDto? dto = mapper.Map<RolePermissionDto>(permission);
            logger.LogInformation("Permission detail retrieved: {PermissionId}", permissionId);
            return dto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetDetailAsync");
            throw;
        }
    }

    public async Task<RolePermissionDto?> GetByCodeAsync(string code, CancellationToken token = default)
    {
        try
        {
            Permission? permission = await readDbContext.Permissions
                .FirstOrDefaultAsync(t => t.Code.ToLower() ==code.ToLower(), token);

            if (permission is null)
            {
                logger.LogWarning("Permission not found: {Code}", code);
                return null;
            }
            RolePermissionDto? dto = mapper.Map<RolePermissionDto>(permission);
            logger.LogInformation("Permission detail retrieved: {Code}", code);
            return dto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetDetailAsync");
            throw;
        }
    }

    public async Task<List<RolePermissionDto>>
        GetPagedAsync(CancellationToken token = default)
    {
        try
        {

            IQueryable<Permission> query = readDbContext.Permissions
                .AsNoTracking()
                .AsQueryable();

            List<RolePermissionDto> items = await query
                .OrderByDescending(t => t.Code)
                .ThenBy(t => t.Description)
                .ProjectTo<RolePermissionDto>(mapper.ConfigurationProvider)
                .ToListAsync(token);

            return (items);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPagedAsync");
            throw;
        }
    }

    public async Task<(IReadOnlyList<RolePermissionDto> Items, int Total)> SearchAsync(string searchTerm, int pageNumber, int pageSize, CancellationToken token = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);

            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Invalid pagination parameters");

            logger.LogInformation(
                "Searching Permissions - Term: {SearchTerm}", searchTerm);


            IQueryable<Permission> query = readDbContext.Permissions;


            var total = await query.CountAsync(token);

            if (total == 0)
            {
                logger.LogInformation(
                    "No Permissions found for search term: {SearchTerm}",
                    searchTerm);
                return (new List<RolePermissionDto>(), 0);
            }
            var term = searchTerm.ToLower().Trim();

            var items = (await query
                .OrderByDescending(t => t.Code)
                .ThenBy(t => t.Description)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<RolePermissionDto>(mapper.ConfigurationProvider)
                .ToListAsync(token))
                .Where(t => t.Code.ToLower().Contains(term))
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
}
