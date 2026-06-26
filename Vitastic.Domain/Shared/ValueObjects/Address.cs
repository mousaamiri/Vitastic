using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.ValueObjects;


public sealed class Address : ValueObject<string>
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string Country { get; }
    public string PostalCode { get; }
    public string? AdditionalInfo { get; }

    // Constants
    public const int MinStreetLength = 5;
    public const int MaxStreetLength = 500;
    public const int MinCityLength = 2;
    public const int MaxCityLength = 100;
    public const int MinStateLength = 2;
    public const int MaxStateLength = 100;
    public const int MinCountryLength = 2;
    public const int MaxCountryLength = 100;
    public const int PostalCodeLength = 10; // Iran: 1234567890
    public const int MaxAdditionalInfoLength = 200;

    private Address(
        string street,
        string city,
        string state,
        string country,
        string postalCode,
        string? additionalInfo):base($"{country} {state} {city} {street} {postalCode}")
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        AdditionalInfo = additionalInfo;
    }

    /// <summary>
    /// Create full address
    /// </summary>
    public static Result<Address> Create(
        string street,
        string city,
        string state,
        string country,
        string postalCode,
        string? additionalInfo = null)
    {
        // Street validation
        if (string.IsNullOrWhiteSpace(street))
            return AddressErrors.StreetEmpty;

        var trimmedStreet = street.Trim();
        if (trimmedStreet.Length < MinStreetLength || trimmedStreet.Length > MaxStreetLength)
            return
                AddressErrors.StreetInvalidLength(MinStreetLength, MaxStreetLength);

        // City validation
        if (string.IsNullOrWhiteSpace(city))
            return AddressErrors.CityEmpty;

        var trimmedCity = city.Trim();
        if (trimmedCity.Length < MinCityLength || trimmedCity.Length > MaxCityLength)
            return
                AddressErrors.CityInvalidLength(MinCityLength, MaxCityLength);

        // State validation
        if (string.IsNullOrWhiteSpace(state))
            return AddressErrors.StateEmpty;

        var trimmedState = state.Trim();
        if (trimmedState.Length < MinStateLength || trimmedState.Length > MaxStateLength)
            return
                AddressErrors.StateInvalidLength(MinStateLength, MaxStateLength);

        // Country validation
        if (string.IsNullOrWhiteSpace(country))
            return AddressErrors.CountryEmpty;

        var trimmedCountry = country.Trim();
        if (trimmedCountry.Length < MinCountryLength || trimmedCountry.Length > MaxCountryLength)
            return
                AddressErrors.CountryInvalidLength(MinCountryLength, MaxCountryLength);

        // Postal code validation
        var postalCodeResult = ValidatePostalCode(postalCode);
        if (postalCodeResult.IsFailure)
            return postalCodeResult.Error;

        var cleanedPostalCode = postalCodeResult.Value;

        // Additional info validation
        string? trimmedAdditionalInfo = null;
        if (!string.IsNullOrWhiteSpace(additionalInfo))
        {
            trimmedAdditionalInfo = additionalInfo.Trim();
            if (trimmedAdditionalInfo.Length > MaxAdditionalInfoLength)
                return
                    AddressErrors.AdditionalInfoTooLong(MaxAdditionalInfoLength);
        }

        return Result.Success(new Address(
            trimmedStreet,
            trimmedCity,
            trimmedState,
            trimmedCountry,
            cleanedPostalCode,
            trimmedAdditionalInfo
        ));
    }

    /// <summary>
    /// Create an Iranian address (default country is Iran)
    /// </summary>
    public static Result<Address> CreateIranian(
        string street,
        string city,
        string state,
        string postalCode,
        string? additionalInfo = null)
    {
        return Create(street, city, state, "ایران", postalCode, additionalInfo);
    }

    private static Result<string> ValidatePostalCode(string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return AddressErrors.PostalCodeEmpty;

        // Remove spaces and dashes
        var cleaned = postalCode.Replace(" ", "").Replace("-", "");

        // Convert Persian numbers to English
        cleaned = cleaned
            .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2')
            .Replace('۳', '3').Replace('۴', '4').Replace('۵', '5')
            .Replace('۶', '6').Replace('۷', '7').Replace('۸', '8')
            .Replace('۹', '9');

        if (cleaned.Length != PostalCodeLength)
            return
                AddressErrors.PostalCodeInvalidLength(PostalCodeLength);

        if (!cleaned.All(char.IsDigit))
            return AddressErrors.PostalCodeInvalidFormat;

        return Result.Success(cleaned);
    }

    /// <summary>
    /// Full address format
    /// </summary>
    public string GetFullAddress()
    {
        var parts = new List<string> { Street, City, State, Country, PostalCode };

        if (!string.IsNullOrWhiteSpace(AdditionalInfo))
            parts.Insert(1, AdditionalInfo);

        return string.Join(", ", parts);
    }

    /// <summary>
    /// Display zip code format (1234567890 → 1234567890)
    /// </summary>
    public string GetFormattedPostalCode()
    {
        if (PostalCode.Length == 10)
        {
            return $"{PostalCode.Substring(0, 5)}-{PostalCode.Substring(5)}";
        }
        return PostalCode;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return PostalCode;
    }

    public override string ToString() => GetFullAddress();
}

public static class AddressErrors
{
    public static Error StreetEmpty => Error.Validation(
        "Address.StreetEmpty",
        "آدرس خیابان نمی‌تواند خالی باشد");

    public static Error StreetInvalidLength(int min, int max) => Error.Validation(
        "Address.StreetInvalidLength",
        $"آدرس خیابان باید بین {min} تا {max} کاراکتر باشد");

    public static Error CityEmpty => Error.Validation(
        "Address.CityEmpty",
        "نام شهر نمی‌تواند خالی باشد");

    public static Error CityInvalidLength(int min, int max) => Error.Validation(
        "Address.CityInvalidLength",
        $"نام شهر باید بین {min} تا {max} کاراکتر باشد");

    public static Error StateEmpty => Error.Validation(
        "Address.StateEmpty",
        "نام استان نمی‌تواند خالی باشد");

    public static Error StateInvalidLength(int min, int max) => Error.Validation(
        "Address.StateInvalidLength",
        $"نام استان باید بین {min} تا {max} کاراکتر باشد");

    public static Error CountryEmpty => Error.Validation(
        "Address.CountryEmpty",
        "نام کشور نمی‌تواند خالی باشد");

    public static Error CountryInvalidLength(int min, int max) => Error.Validation(
        "Address.CountryInvalidLength",
        $"نام کشور باید بین {min} تا {max} کاراکتر باشد");

    public static Error PostalCodeEmpty => Error.Validation(
        "Address.PostalCodeEmpty",
        "کدپستی نمی‌تواند خالی باشد");

    public static Error PostalCodeInvalidLength(int length) => Error.Validation(
        "Address.PostalCodeInvalidLength",
        $"کدپستی باید دقیقاً {length} رقم باشد");

    public static Error PostalCodeInvalidFormat => Error.Validation(
        "Address.PostalCodeInvalidFormat",
        "کدپستی باید فقط شامل اعداد باشد");

    public static Error AdditionalInfoTooLong(int max) => Error.Validation(
        "Address.AdditionalInfoTooLong",
        $"اطلاعات تکمیلی نباید بیشتر از {max} کاراکتر باشد");
}
