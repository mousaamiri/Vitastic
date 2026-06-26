using Vitastic.Domain.Entities.Roles.Events;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Roles;

public class Role : AggregateRoot<RoleId>
{
    // ------------------------
    // Properties | Permission is a Value Object
    // ------------------------
    private readonly HashSet<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    // Simple property
    public RoleName Name { get; private set; }

    // ------------------------
    // Constructors
    // ------------------------
    private Role(RoleId id, RoleName name) : base(id)
    {
        Name = name;
    }
    //For Ef core
    protected Role()
    { }
    //-------------------------
    // Factory method
    //-------------------------
    public static Result<Role> Create(string roleName)
    {
        Result<RoleName> nameResult = RoleName.Create(roleName);
        if (nameResult.IsFailure)
            return nameResult.Error;
        var role = new Role(RoleId.New(), nameResult.Value);
        role.RaiseDomainEvent( RoleCreatedDomainEvent.Create(role.Id, role.Name));
        return role;
    }
    // ------------------------
    // Behaviors
    // ------------------------
    public Result UpdateName(RoleName roleName)
    {
        Name = roleName;
        return Result.Success();
    }

    public bool HasPermission(RolePermission rolePermission)
        => _rolePermissions.Contains(rolePermission);

    public Result ManagerPermissions(List<PermissionId> permissionIds)
    {
        // Validation
        if (permissionIds == null)
            return Result.Failure(Error.NullValue);

        // Remove duplicates
        var uniquePermissionIds = permissionIds.Distinct().ToHashSet();
        var currentPermissionIds = _rolePermissions.Select(ur => ur.PermissionId).ToHashSet();

        // Remove roles
        IEnumerable<PermissionId> permissionToRemove = currentPermissionIds.Except(uniquePermissionIds);
        _rolePermissions.RemoveWhere(ur => permissionToRemove.Contains(ur.PermissionId));

        // Add new roles
        IEnumerable<PermissionId> permissionToAdd = uniquePermissionIds.Except(currentPermissionIds);
        foreach (PermissionId permissionId in permissionToAdd)
        {
            _rolePermissions.Add(RolePermission.Create(Id,permissionId));
        }

        return Result.Success();
    }
}

public static class RoleErrors
{
    public static Error NullPermission => Error.Validation("Role.NullPermission", "مجوز نمی تواند null باشد.");
    public static Error RoleNotFound => Error.NotFound("Role.RoleNotFound", "نقش یافت نشد.");
    public static Error InvalidRoleId => Error.NotFound("Role.InvalidRoleId", "شناسه نقش نامعتبر است.");

    public static Error PermissionAlreadyExists (RolePermissionId rolePermissionId )=>
        Error.Validation("Role.PermissionAlreadyExists", $"مجوز {rolePermissionId.Value} از قبل در این نقش وجود دارد.");

    public static Error PermissionNotFound (RolePermissionId rolePermissionId )=>
  Error.Validation("Role.PermissionNotFound", $"مجوز {rolePermissionId.Value} در این نقش یافت نشد.");
}
