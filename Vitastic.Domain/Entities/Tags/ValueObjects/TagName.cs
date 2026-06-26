using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Tags.ValueObjects
{
    public sealed class TagName : ValueObject<string>
    {
        public const int MinLength = 2;
        public const int MaxLength = 50;

        private TagName(string value) : base(value) { }

        public static Result<TagName> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return TagNameErrors.Empty;

            var trimmed = value.Trim();

            if (trimmed.Length < MinLength)
                return TagNameErrors.TooShort(MinLength);

            if (trimmed.Length > MaxLength)
                return TagNameErrors.TooLong(MaxLength);

            return new TagName(trimmed);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }

        public override string ToString() => Value;
    }

    public static class TagNameErrors
    {
        public static Error Empty => Error.Validation(
            "TagName.Empty",
            "نام برچسب نمی‌تواند خالی باشد");

        public static Error TooShort(int min) => Error.Validation(
            "TagName.TooShort",
            $"نام برچسب باید حداقل {min} کاراکتر باشد");

        public static Error TooLong(int max) => Error.Validation(
            "TagName.TooLong",
            $"نام برچسب نباید بیشتر از {max} کاراکتر باشد");
    }
}
