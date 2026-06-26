namespace Vitastic.Api.Features.Permissions.Responses;

public sealed record PermissionResponse(Guid Id, string Code, string? Description);
