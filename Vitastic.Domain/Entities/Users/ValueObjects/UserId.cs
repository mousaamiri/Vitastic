using System.Text.Json.Serialization;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public sealed class UserId:GuidBasedId<UserId>
{
    //Constructor
    private UserId(Guid value) : base(value) { }
    //Override
    protected override UserId Create(Guid value) => new(value);
    public static UserId New() => new(Guid.NewGuid());
    public static Result<UserId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new UserId(guid), BaseIdErrors.Empty);
    public static Result<UserId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new UserId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}

