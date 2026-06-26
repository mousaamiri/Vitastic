using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public sealed class UserRoleId:GuidBasedId<UserRoleId>
{
    //Constructor
    private UserRoleId(Guid value) : base(value) { }
    //Override
    protected override UserRoleId Create(Guid value) => new(value);
    public static UserRoleId New() => new(Guid.NewGuid());
    public static Result<UserRoleId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new UserRoleId(guid), BaseIdErrors.Empty);
    public static Result<UserRoleId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new UserRoleId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}

