using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Roles.ValueObjects;

public sealed class RoleName:ValueObject<string>
{
    //Fields
    private const int MinLenght = 3;
    public const int MaxLenght = 100;
    public const string Pattern = @"^[a-zA-Z0-9\s]+$";
    //Constructor
    private RoleName(string value) : base(value) { }
    //Factory method
    public static Result<RoleName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return RoleNameErrors.NameEmpty;
        if(value.Length is < MinLenght or > MaxLenght)
            return RoleNameErrors.ValidNameLength(MinLenght,MaxLenght);
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, Pattern))
            return RoleNameErrors.InvalidFormat;
        return new RoleName(value);
    }
    //For equality check
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    //To string
    public override string ToString() => Value;
    // Implicit conversion to string
    public static implicit operator string(RoleName roleName) => roleName.Value;
}
public static class RoleNameErrors
{
    public static Error NameEmpty
        => Error.Validation("RoleName.Empty", "نام نقش نمی‌تواند خالی باشد.");

    public static Error ValidNameLength(int minLenght,int maxLenght)
        => Error.Validation("RoleName.InvalidLength", $"نام نقش باید بین {minLenght} و {maxLenght} کاراکتر باشد.");
    public static Error InvalidFormat =>
        Error.Validation("RoleName.InvalidFormat", "نام نقش فقط می‌تواند شامل حروف، اعداد و فاصله باشد.");
}
