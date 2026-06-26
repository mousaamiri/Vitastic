using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Roles.ValueObjects;

public sealed class RoleId : GuidBasedId<RoleId>
{
    //Constructor
    private RoleId(Guid value) : base(value)
    {
    }

    //Override
    protected override RoleId Create(Guid value) => new(value);
    public static RoleId New() => new(Guid.NewGuid());

    public static Result<RoleId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new RoleId(guid), BaseIdErrors.Empty);

    public static Result<RoleId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new RoleId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
    //ToString
    public override string ToString()  => Value.ToString();

}

/*
 * 	public sealed class RoleId:IntBasedId<RoleId>
    {
        //Constructor
        private RoleId(int value) : base(value) { }
        //Override
        protected override RoleId Create(int value) => new(value);
        public static Result<RoleId> CreateFrom(int value) =>
            CreateFrom(value, id => new RoleId(id), BaseIdErrors.Invalid);
        public static Result<RoleId> CreateFrom(string value) =>
            CreateFrom(
                value,
                guid => new RoleId(guid),
                BaseIdErrors.Empty,
                BaseIdErrors.InvalidFormat(value));
    }

 */
