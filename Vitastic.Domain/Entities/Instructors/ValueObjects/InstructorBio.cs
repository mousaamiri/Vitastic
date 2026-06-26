using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Instructors.ValueObjects;


public sealed class InstructorBio : ValueObject<string>
{

    public const int MaxLength = 1000;
    public const int MinLength = 10;

    private InstructorBio(string value) : base(value) { }

    // Factory method with validation
    public static Result<InstructorBio> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return InstructorBioErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            return InstructorBioErrors.TooShort(MinLength);

        if (trimmed.Length > MaxLength)
            return InstructorBioErrors.TooLong(MaxLength);

        return Result.Success(new InstructorBio(trimmed));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

public static class InstructorBioErrors
{
    public static Error Empty => Error.Validation(
        "InstructorBio.Empty",
        "بیوگرافی مدرس نمی‌تواند خالی باشد");

    public static Error TooShort(int minLength) => Error.Validation(
        "InstructorBio.TooShort",
        $"بیوگرافی باید حداقل {minLength} کاراکتر باشد");

    public static Error TooLong(int maxLength) => Error.Validation(
        "InstructorBio.TooLong",
        $"بیوگرافی نباید بیشتر از {maxLength} کاراکتر باشد");
}
