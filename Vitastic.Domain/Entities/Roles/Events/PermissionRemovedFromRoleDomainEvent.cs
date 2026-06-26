using System.Text.Json.Serialization;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Events;

namespace Vitastic.Domain.Entities.Roles.Events;

public sealed record PermissionRemovedFromRoleDomainEvent : DomainEvent
{
    public Guid RoleId { get; init; }
    public Guid RolePermissionId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public PermissionRemovedFromRoleDomainEvent(Guid roleId, Guid rolePermissionId)
    {
        RoleId = roleId;
        RolePermissionId = rolePermissionId;
    }

    public static PermissionRemovedFromRoleDomainEvent Create(RoleId roleId, RolePermissionId rolePermissionId)
        => new(roleId.Value, rolePermissionId.Value);
}

public sealed record PermissionAddedToRoleDomainEvent : DomainEvent
{
    public Guid RoleId { get; init; }
    public Guid RolePermissionId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public PermissionAddedToRoleDomainEvent(Guid roleId, Guid rolePermissionId)
    {
        RoleId = roleId;
        RolePermissionId = rolePermissionId;
    }

    public static PermissionAddedToRoleDomainEvent Create(RoleId roleId, RolePermissionId RolePermissionId)
        => new(roleId.Value, RolePermissionId.Value);
}

public sealed record RoleCreatedDomainEvent : DomainEvent
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public RoleCreatedDomainEvent(Guid roleId, string roleName)
    {
        RoleId = roleId;
        RoleName = roleName;
    }

    public static RoleCreatedDomainEvent Create(RoleId roleId, RoleName roleName)
        => new(roleId.Value, roleName.Value);
}
