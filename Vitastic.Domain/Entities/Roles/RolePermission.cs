using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Domain.Entities.Roles;

public class RolePermission:BaseEntity<RolePermissionId>
{
    public RoleId RoleId { get; private set; }
    public PermissionId PermissionId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }=DateTimeOffset.UtcNow;

    public RolePermission() { }//For ef
    private RolePermission(RolePermissionId id, RoleId roleId, PermissionId permissionId)
        : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
    public static RolePermission Create(RoleId roleId, PermissionId permissionId)
    {
        return new RolePermission(RolePermissionId.New(), roleId, permissionId);
    }
}

