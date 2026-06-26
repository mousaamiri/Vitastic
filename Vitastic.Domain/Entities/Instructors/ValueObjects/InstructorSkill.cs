using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Instructors.ValueObjects;

public sealed class InstructorSkill : ValueObject<string>
{
    public const int MaxLength = 50;
    private const int MinLength = 2;

    private InstructorSkill(string value) : base(value) { }

    // Factory method with validation
    public static Result<InstructorSkill> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return InstructorSkillErrors.Empty;

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            return InstructorSkillErrors.TooShort(MinLength);

        if (trimmed.Length > MaxLength)
            return InstructorSkillErrors.TooLong(MaxLength);

        return Result.Success(new InstructorSkill(trimmed));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(InstructorSkill skill) => skill.Value;
}

public static class InstructorSkillErrors
{
    public static Error Empty => Error.Validation(
        "InstructorSkill.Empty",
        "نام مهارت نمی‌تواند خالی باشد");

    public static Error TooShort(int minLength) => Error.Validation(
        "InstructorSkill.TooShort",
        $"نام مهارت باید حداقل {minLength} کاراکتر باشد");

    public static Error TooLong(int maxLength) => Error.Validation(
        "InstructorSkill.TooLong",
        $"نام مهارت نباید بیشتر از {maxLength} کاراکتر باشد");
}
