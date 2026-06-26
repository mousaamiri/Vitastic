using System.Runtime.InteropServices.JavaScript;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;

public sealed class Description:ValueObject<string>
{
    //Private Fields
    public const int MaxLength = 2000;
    public const int MinLength = 10;

    //Constructor
    private Description(string value) : base(value) { }
     //Factory Methods
    public static Result<Description> Create(string value)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrWhiteSpace(value))
            return DescriptionErrors.Empty;
        if (trimmed.Length > MaxLength)
            return DescriptionErrors.TooLong(MaxLength);
        if (trimmed.Length < MinLength)
            return DescriptionErrors.TooShort(MinLength);
        return new Description(trimmed);

    }
    //Equality
    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
    //Converter
    public static implicit operator string(Description description) => description.Value;
}
public static class DescriptionErrors
{
    public static Error Empty=> Error.Validation("Description.Empty","توضیحات نمی تواند خالی باشد.");
    public static Error TooLong(int maxLength) => Error.Validation(
        "Description.TooLong",
        $"توضیحات نمی تواند بیشتر از {maxLength} کاراکتر باشد.");
    public static Error TooShort(int minLength) => Error.Validation(
        "Description.TooShort",
        $"توضیحات نمی تواند کمتر از {minLength} کاراکتر باشد.");

}
