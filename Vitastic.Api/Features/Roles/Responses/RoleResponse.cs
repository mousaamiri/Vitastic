namespace Vitastic.Api.Features.Roles.Responses;

public sealed record RoleResponse(
    Guid Id,
    string Name,
    int PermissionCount);

public sealed record RoleDetailResponse(
    Guid Id,
    string Name,
    List<RolePermissionResponse> Permissions);

public sealed record RolePermissionResponse(
    Guid Id,
    string Code,
    string Description);
