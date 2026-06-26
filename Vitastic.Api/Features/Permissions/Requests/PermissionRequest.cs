namespace Vitastic.Api.Features.Permissions.Requests;

public sealed record CreatePermissionRequest(string Code,string Description);
public sealed record UpdatePermissionRequest(string Code,string Description);
