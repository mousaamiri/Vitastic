using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Roles.ValueObjects;

public sealed class PermissionId : GuidBasedId<PermissionId>
{
    //Constructor
    private PermissionId(Guid value) : base(value)
    {
    }

    //Override
    protected override PermissionId Create(Guid value) => new(value);
    public static PermissionId New() => new(Guid.NewGuid());

    public static Result<PermissionId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new PermissionId(guid), BaseIdErrors.Empty);

    public static Result<PermissionId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new PermissionId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
    //ToString
    public override string ToString()  => Value.ToString();

}
