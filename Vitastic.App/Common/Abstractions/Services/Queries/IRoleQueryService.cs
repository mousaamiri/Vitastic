using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.App.Features.Roles.Dtos;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

public interface IRoleQueryService
{
    Task<RoleDetailDto?> GetById(RoleId roleId, CancellationToken token=default);
    Task<RoleDto?> FindByNameAsync(RoleName roleId, CancellationToken token=default);
    Task<(IReadOnlyList<RoleDto> items, int total)> GetPagedAsync(
        string? searchTerm,
        int pageNumber, int pageSize, CancellationToken token=default);
    Task<List<RolePermissionDto>> GetRolePermissionsAsync(RoleId roleId, CancellationToken token=default);
    Task<bool> HasPermissionAsync(RoleId roleId, PermissionId permissionId, CancellationToken token=default);
    Task<bool> HasPermissionAsync(RoleId roleId, string permissionCode, CancellationToken token=default);
}
