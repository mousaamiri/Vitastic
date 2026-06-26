using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Roles.ValueObjects;

public sealed class RolePermissionId : GuidBasedId<RolePermissionId>
{
    //Constructor
    private RolePermissionId(Guid value) : base(value)
    {
    }

    //Override
    protected override RolePermissionId Create(Guid value) => new(value);
    public static RolePermissionId New() => new(Guid.NewGuid());

    public static Result<RolePermissionId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new RolePermissionId(guid), BaseIdErrors.Empty);

    public static Result<RolePermissionId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new RolePermissionId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
    //ToString
    public override string ToString()  => Value.ToString();

}
