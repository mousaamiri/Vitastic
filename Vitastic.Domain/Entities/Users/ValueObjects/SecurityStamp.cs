using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public class SecurityStamp:ValueObject<Guid>
{

    //Fields
    public const int Length = 36;
    //Constructor
    private SecurityStamp(Guid value):base(value) { }
    //Generate new security stamp
    public static SecurityStamp Generate() => new(Guid.NewGuid());
    //Regenerate security stamp from database
    public static Result<SecurityStamp> Create(Guid value)
    {
        if (value == Guid.Empty)
            return SecurityStampErrors.EmptySecurityStamp;
        return new SecurityStamp(value);
    }
    //Regenerate - to invalidate sessions
    public SecurityStamp Regenerate() => Generate();
    // Get a new security stamp
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    //ToString
    public override string ToString()
        => Value.ToString("N");
}
public static class SecurityStampErrors
{
    public static Error EmptySecurityStamp =>
        Error.Validation("SecurityStamp.Empty", "سکیوریتی استمپ نمی‌تواند خالی باشد.");

}
