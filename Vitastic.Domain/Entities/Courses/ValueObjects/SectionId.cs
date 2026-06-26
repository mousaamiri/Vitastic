using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Courses.ValueObjects;

public sealed class SectionId: GuidBasedId<SectionId>
{
    //Constructor
    private SectionId(Guid value) : base(value) { }
    //Override
    protected override SectionId Create(Guid value) => new(value);
    public static SectionId New() => new(Guid.NewGuid());
    public static Result<SectionId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new SectionId(guid), BaseIdErrors.Empty);
    public static Result<SectionId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new SectionId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
