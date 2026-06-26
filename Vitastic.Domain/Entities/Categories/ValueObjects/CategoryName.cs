// Domain/Categories/ValueObjects/CategoryName.cs

using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Categories.ValueObjects
{
    public sealed class CategoryName : ValueObject<string>
    {

        public const int MinLength = 2;
        public const int MaxLength = 100;

        private CategoryName(string value) : base(value) { }

        public static Result<CategoryName> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return CategoryNameErrors.Empty;

            var trimmed = value.Trim();

            if (trimmed.Length < MinLength)
                return CategoryNameErrors.TooShort(MinLength);

            if (trimmed.Length > MaxLength)
                return CategoryNameErrors.TooLong(MaxLength);

            return new CategoryName(trimmed);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
// در کلاس CategoryName این متد را اضافه کنید
        public bool HasTerm(string term)
        {
            return Value.Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString() => Value;
    }

    public static class CategoryNameErrors
    {
        public static Error Empty => Error.Validation(
            "CategoryName.Empty",
            "نام دسته‌بندی نمی‌تواند خالی باشد");

        public static Error TooShort(int min) => Error.Validation(
            "CategoryName.TooShort",
            $"نام دسته‌بندی باید حداقل {min} کاراکتر باشد");

        public static Error TooLong(int max) => Error.Validation(
            "CategoryName.TooLong",
            $"نام دسته‌بندی نباید بیشتر از {max} کاراکتر باشد");
    }
}
