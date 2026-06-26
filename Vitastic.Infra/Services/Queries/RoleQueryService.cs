using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.App.Features.Roles.Dtos;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries
{
    internal class RoleQueryService(
        string? connectionString,
        ApplicationWriteDbContext readDbContext,
        IMapper mapper,
        ILogger<RoleQueryService> logger) : IRoleQueryService
    {
        public async Task<bool> HasPermissionAsync(RoleId roleId, PermissionId permissionId,
            CancellationToken token = default)
        {
            try
            {
                return await readDbContext.RolePermissions
                    .AnyAsync(r => r.RoleId == roleId
                                   && r.PermissionId == permissionId, token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in HasPermission");
                throw;
            }
        }

        public async Task<bool> HasPermissionAsync(RoleId roleId, string permissionCode, CancellationToken token = default)
        {
            try
            {
                PermissionId? permissionId = await readDbContext
                    .Permissions
                    .Where(r => r.Code==permissionCode)
                    .Select(r=>r.Id)
                    .FirstOrDefaultAsync(token);
                if(permissionId is null)
                    return false;
                return await readDbContext.RolePermissions
                    .AnyAsync(r => r.RoleId == roleId
                                   && r.PermissionId == permissionId, token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in HasPermission");
                throw;
            }
        }

        public async Task<RoleDetailDto?> GetById(RoleId roleId, CancellationToken token = default)
        {
            try
            {
                Role? role = await readDbContext.Roles
                    .FirstOrDefaultAsync(r => r.Id == roleId, token);
                if (role is null)
                    return null;
                List<PermissionId> permissionIds = await readDbContext.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync(token);
                List<RolePermissionDto> permissions = await readDbContext.Permissions
                    .Where(p => permissionIds.Contains(p.Id))
                    .ProjectTo<RolePermissionDto>(mapper.ConfigurationProvider)
                    .ToListAsync(token);

                return new RoleDetailDto(role.Id.Value, role.Name.Value, permissions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetById");
                throw;
            }
        }

        public async Task<RoleDto?> FindByNameAsync(RoleName roleName, CancellationToken token = default)
        {
            try
            {
                var role = await readDbContext.Roles
                    .FirstOrDefaultAsync(r => r.Name == roleName, token);

                if (role is null)
                    return null;

                var permissionCount = await readDbContext.RolePermissions
                    .CountAsync(rp => rp.RoleId == role.Id, token);

                return new RoleDto(role.Id.Value, role.Name.Value, permissionCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in FindByNameAsync");
                throw;
            }
        }

        public async Task<List<RolePermissionDto>> GetRolePermissionsAsync(RoleId roleId,
             CancellationToken token = default)
        {
            try
            {
                var query = readDbContext.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Join(readDbContext.Permissions, rp => rp.PermissionId, p => p.Id, (rp, p) => p);

                var items = await query
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

        public async Task<(IReadOnlyList<RoleDto> items, int total)> GetPagedAsync(string? searchTerm,int pageNumber, int pageSize,
            CancellationToken token = default)
        {
            try
            {
                var term = searchTerm?.Trim();
                if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                    throw new ArgumentException("Invalid pagination parameters");

                IQueryable<Role>? roleQuery=readDbContext.Roles.AsNoTracking();
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var normalizedSearchTerm = $"%{term}%";
                    roleQuery = readDbContext.Roles
                        .FromSqlRaw(@"SELECT * FROM ""Roles"" WHERE UPPER(""Name"") LIKE UPPER(@searchTerm)"
                            , new NpgsqlParameter("@searchTerm", normalizedSearchTerm));
                }
                var total = await roleQuery.CountAsync(token);

                var roleIds = await roleQuery
                    .OrderByDescending(r => r.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => r.Id)
                    .ToListAsync(token);

                var permissionCounts = await readDbContext.RolePermissions
                    .Where(rp => roleIds.Contains(rp.RoleId))
                    .GroupBy(rp => rp.RoleId)
                    .Select(g => new { RoleId = g.Key, Count = g.Count() })
                    .ToListAsync(token);
                var permissionCountDict = permissionCounts.ToDictionary(pc => pc.RoleId, pc => pc.Count);

                List<RoleDto> items = await roleQuery
                    .Where(r => roleIds.Contains(r.Id))
                    .Select(r => new RoleDto
                    (
                        r.Id.Value,
                        r.Name.Value,
                        permissionCountDict.GetValueOrDefault(r.Id,0)
                    ))
                    .ToListAsync(token);

                logger.LogInformation(
                    "Listed {Count} roles - Page {Page}/{Total}",
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
    }
}
