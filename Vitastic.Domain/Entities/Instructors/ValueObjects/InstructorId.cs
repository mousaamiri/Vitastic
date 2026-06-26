using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Instructors.ValueObjects;

public sealed class InstructorId: GuidBasedId<InstructorId>
{
    //Constructor
    private InstructorId(Guid value) : base(value) { }
    //Override
    protected override InstructorId Create(Guid value) => new(value);
    public static InstructorId New() => new(Guid.NewGuid());
    public static Result<InstructorId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new InstructorId(guid), BaseIdErrors.Empty);
    public static Result<InstructorId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new InstructorId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}

