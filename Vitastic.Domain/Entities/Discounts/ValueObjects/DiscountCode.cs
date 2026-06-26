using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Discounts.ValueObjects;

public sealed class DiscountCode : ValueObject<string>
{
    public const int MinLength = 3;
    public const int MaxLength = 50;
    public const string Pattern = @"^[A-Z0-9_\-]+$";

    private DiscountCode(string value) : base(value) { }

    public static Result<DiscountCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DiscountCodeErrors.Empty;

        var trimmed = value.Trim().ToUpperInvariant();

        if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
            return
                DiscountCodeErrors.InvalidLength(MinLength, MaxLength);

        // Only letters, numbers and dashes
        if (!System.Text.RegularExpressions.Regex.IsMatch(trimmed, Pattern))
            return DiscountCodeErrors.InvalidFormat;

        return Result.Success(new DiscountCode(trimmed));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(DiscountCode code) => code.Value;
}

public static class DiscountCodeErrors
{
    public static Error Empty => Error.Validation(
        "DiscountCode.Empty",
        "کد تخفیف نمی‌تواند خالی باشد");

    public static Error InvalidLength(int min, int max) => Error.Validation(
        "DiscountCode.InvalidLength",
        $"کد تخفیف باید بین {min} تا {max} کاراکتر باشد");

    public static Error InvalidFormat => Error.Validation(
        "DiscountCode.InvalidFormat",
        "کد تخفیف فقط می‌تواند شامل حروف انگلیسی، اعداد و خط تیره باشد");
}
