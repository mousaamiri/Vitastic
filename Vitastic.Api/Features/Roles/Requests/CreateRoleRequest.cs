namespace Vitastic.Api.Features.Roles.Requests;
public sealed record CreateRoleRequest(string RoleName, List<Guid> PermissionIds);
public sealed record UpdateRoleNameRequest(string RoleName);
public sealed record UpdateRoleRequest(string RoleName, List<Guid> PermissionIds);
public sealed record AddPermissionToRoleRequest(Guid PermissionId);
