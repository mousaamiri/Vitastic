namespace Vitastic.Web.Models.DTOs.Role;

public sealed record RoleDetailDto(
    Guid Id,
    string Name,
    List<RolePermissionDto> Permissions);
public record RolePermissionDto(Guid Id, string Code, string? Description);
public record UpsertRoleRequest(string RoleName, List<string> PermissionIds);

public sealed class RoleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int PermissionCount { get; init; }

    public RoleDto(Guid id, string name, int permissionCount)
    {
        Id = id;
        Name = name;
        PermissionCount = permissionCount;
    }

    public RoleDto() { }
}
