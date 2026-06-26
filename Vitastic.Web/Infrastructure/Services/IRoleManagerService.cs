using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Role;

namespace Vitastic.Web.Infrastructure.Services;

public interface IRoleManagerService
{
    Task<PaginatedApiResponse<RoleDto>> GetRolesAsync(string? searchTerm,int pageNumber = 1,int pageSize = 10, CancellationToken ct=default);
    Task<ApiResponse<RoleDetailDto>> GetRoleAsync(Guid roleId,CancellationToken ct=default);
    Task<ApiResponse<List<RolePermissionDto>>> GetRolePermissionAsync(Guid roleId,CancellationToken ct=default);
    Task<ApiResponse<List<RolePermissionDto>>> GetAllPermissionAsync(CancellationToken ct=default);
    Task<ApiResponse> UpdateRoleAsync(Guid roleId, UpsertRoleRequest request, CancellationToken ct = default);
    Task<ApiResponse<Guid>> CreateRoleAsync(UpsertRoleRequest request, CancellationToken ct = default);
}

public sealed class RoleManagerService(IApiClient apiClient) : IRoleManagerService
{
    public async Task<PaginatedApiResponse<RoleDto>> GetRolesAsync(string? searchTerm, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
        => await apiClient.GetPaginatedAsync<RoleDto>("Roles",new{searchTerm,pageNumber,pageSize}, ct: ct);

    public async Task<ApiResponse<RoleDetailDto>> GetRoleAsync(Guid roleId, CancellationToken ct = default)
        => await apiClient.GetAsync<RoleDetailDto>($"Roles/{roleId}", ct: ct);

    public async Task<ApiResponse<List<RolePermissionDto>>> GetRolePermissionAsync(Guid roleId, CancellationToken ct = default)
        => await apiClient.GetAsync<List<RolePermissionDto>>($"Roles/{roleId}/permissions", ct: ct);

    public async Task<ApiResponse<List<RolePermissionDto>>> GetAllPermissionAsync(CancellationToken ct = default)
        => await apiClient.GetAsync<List<RolePermissionDto>>($"Permissions", ct: ct);

    public async Task<ApiResponse> UpdateRoleAsync(Guid roleId, UpsertRoleRequest request, CancellationToken ct = default)
        => await apiClient.PutAsync($"Roles/{roleId}",request, ct: ct);


    public async Task<ApiResponse<Guid>> CreateRoleAsync(UpsertRoleRequest request, CancellationToken ct = default)
        => await apiClient.PostAsync<Guid>($"Roles",request, ct: ct);

}
