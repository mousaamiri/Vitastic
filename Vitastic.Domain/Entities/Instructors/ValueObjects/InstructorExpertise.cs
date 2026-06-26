using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Instructors.ValueObjects;

public sealed class InstructorExpertise : ValueObject<string>
{
    public const int MaxLength = 200;
    public const int MinLength = 2;

    private InstructorExpertise(string value) : base(value) { }

    // Factory method with validation
    public static Result<InstructorExpertise> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return InstructorExpertiseErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            return InstructorExpertiseErrors.TooShort(MinLength);

        if (trimmed.Length > MaxLength)
            return InstructorExpertiseErrors.TooLong(MaxLength);

        return Result.Success(new InstructorExpertise(trimmed));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

public static class InstructorExpertiseErrors
{
    public static Error Empty => Error.Validation(
        "InstructorExpertise.Empty",
        "حوزه کاری مدرس نمی‌تواند خالی باشد");

    public static Error TooShort(int minLength) => Error.Validation(
        "InstructorExpertise.TooShort",
        $"حوزه کاری باید حداقل {minLength} کاراکتر باشد");

    public static Error TooLong(int maxLength) => Error.Validation(
        "InstructorExpertise.TooLong",
        $"حوزه کاری نباید بیشتر از {maxLength} کاراکتر باشد");
}
