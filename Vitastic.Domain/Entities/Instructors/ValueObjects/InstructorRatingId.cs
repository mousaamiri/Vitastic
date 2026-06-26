using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Instructors.ValueObjects;

public sealed class InstructorRatingId: GuidBasedId<InstructorRatingId>
{
    //Constructor
    private InstructorRatingId(Guid value) : base(value) { }
    //Override
    protected override InstructorRatingId Create(Guid value) => new(value);
    public static InstructorRatingId New() => new(Guid.NewGuid());
    public static Result<InstructorRatingId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new InstructorRatingId(guid), BaseIdErrors.Empty);
    public static Result<InstructorRatingId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new InstructorRatingId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}

