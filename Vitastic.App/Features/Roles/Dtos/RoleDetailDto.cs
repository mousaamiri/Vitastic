using Vitastic.App.Features.Permissions.Dtos;

namespace Vitastic.App.Features.Roles.Dtos
{
    /// <summary>
    /// Role with permissions
    /// </summary>
    public sealed record RoleDetailDto(
        Guid Id,
        string Name,
        List<RolePermissionDto> Permissions);
}
