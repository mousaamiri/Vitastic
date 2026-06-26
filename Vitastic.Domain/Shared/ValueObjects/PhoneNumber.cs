using System.Text.RegularExpressions;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;

public sealed class PhoneNumber : ValueObject<string>
{
    public string CountryCode { get; }
    public string Number { get; }

    // Constants
    public const int MinLength = 10;
    public const int MaxLength = 15;
    public const string DefaultCountryCode = "+98"; // Iran

    private PhoneNumber(string value, string countryCode, string number):base(value)
    {
        Value = value;
        CountryCode = countryCode;
        Number = number;
    }

    /// <summary>
    /// Create a phone number with full format (+98 9123456789)
    /// </summary>
    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return PhoneNumberErrors.Empty;

        var cleaned = CleanPhoneNumber(value);

        if (cleaned.Length < MinLength || cleaned.Length > MaxLength)
            return
                PhoneNumberErrors.InvalidLength(MinLength, MaxLength);

        if (!IsValidFormat(cleaned))
            return PhoneNumberErrors.InvalidFormat;

        var (countryCode, number) = ExtractParts(cleaned);

        var formatted = FormatPhoneNumber(countryCode, number);

        return Result.Success(new PhoneNumber(formatted, countryCode, number));
    }

    /// <summary>
    /// Create Iranian phone number (09123456789)
    /// </summary>
    public static Result<PhoneNumber> CreateIranian(string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
            return PhoneNumberErrors.Empty;

        var cleaned = CleanPhoneNumber(mobileNumber);

        // Convert 9123456789 to 09123456789
        if (cleaned.Length == 10 && cleaned.StartsWith("9"))
            cleaned = "0" + cleaned;

        // Convert 09123456789 to +989123456789
        if (cleaned.Length == 11 && cleaned.StartsWith("0"))
            cleaned = "+98" + cleaned.Substring(1);

        if (!cleaned.StartsWith("+98"))
            cleaned = "+98" + cleaned;

        if (cleaned.Length != 13) // +98 + 10 digits
            return PhoneNumberErrors.InvalidIranianMobile;

        // Check the validity of Iranian mobile numbers
        var numberPart = cleaned.Substring(3); // Delete +98
        if (!Regex.IsMatch(numberPart, @"^9[0-9]{9}$"))
            return PhoneNumberErrors.InvalidIranianMobile;

        return Result.Success(new PhoneNumber(cleaned, "+98", numberPart));
    }

    private static string CleanPhoneNumber(string phone)
    {
        // Remove unnecessary characters
        var cleaned = Regex.Replace(phone, @"[\s\-\(\)\.]", "");

        // Convert Persian numbers to English
        cleaned = cleaned
            .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2')
            .Replace('۳', '3').Replace('۴', '4').Replace('۵', '5')
            .Replace('۶', '6').Replace('۷', '7').Replace('۸', '8')
            .Replace('۹', '9');

        return cleaned;
    }

    private static bool IsValidFormat(string phone)
    {
        // Only numbers and + at the beginning
        return Regex.IsMatch(phone, @"^\+?[0-9]+$");
    }

    private static (string countryCode, string number) ExtractParts(string phone)
    {
        if (phone.StartsWith("+98"))
            return ("+98", phone.Substring(3));

        if (phone.StartsWith("+1"))
            return ("+1", phone.Substring(2));
        if (phone.StartsWith("0"))
            return (DefaultCountryCode, phone.Substring(1));
        if (phone.StartsWith("+"))
        {
            var match = Regex.Match(phone, @"^\+(\d{1,3})(\d+)$");
            if (match.Success)
                return ($"+{match.Groups[1].Value}", match.Groups[2].Value);
        }

        return (DefaultCountryCode, phone);
    }

    private static string FormatPhoneNumber(string countryCode, string number)
    {
        return $"{countryCode}{number}";
    }

    /// <summary>
    /// Display format
    /// </summary>
    public string GetDisplayFormat()
    {
        if (string.Equals(CountryCode, "+98", StringComparison.Ordinal) && Number.Length == 10)
        {
            // 0912 345 6789
            return $"0{Number.Substring(0, 3)} {Number.Substring(3, 3)} {Number.Substring(6)}";
        }

        return Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}

public static class PhoneNumberErrors
{
    public static Error Empty => Error.Validation(
        "PhoneNumber.Empty",
        "شماره تلفن نمی‌تواند خالی باشد");

    public static Error InvalidLength(int min, int max) => Error.Validation(
        "PhoneNumber.InvalidLength",
        $"شماره تلفن باید بین {min} تا {max} رقم باشد");

    public static Error InvalidFormat => Error.Validation(
        "PhoneNumber.InvalidFormat",
        "فرمت شماره تلفن نامعتبر است");

    public static Error InvalidIranianMobile => Error.Validation(
        "PhoneNumber.InvalidIranianMobile",
        "شماره موبایل ایرانی نامعتبر است (باید با 09 شروع شود)");
}
