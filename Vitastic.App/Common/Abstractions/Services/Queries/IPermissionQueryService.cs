using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.Domain.Entities.Roles.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

public interface IPermissionQueryService
{
    // ==================== GET SINGLE ====================
    Task<RolePermissionDto?> GetByIdAsync(
        PermissionId permissionId,
        CancellationToken token = default);

    Task<RolePermissionDto?> GetByCodeAsync(
        string code,
        CancellationToken token = default);

    // ==================== LIST ====================
    Task<List<RolePermissionDto>> GetPagedAsync(CancellationToken token = default);

    // ==================== SEARCH ====================
    Task<(IReadOnlyList<RolePermissionDto> Items, int Total)> SearchAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken token = default);
}
