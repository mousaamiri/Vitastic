using Vitastic.Domain.Shared.Helpers;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

/// <summary>
/// Value Object for Password with Hash and Validation
/// </summary>
public sealed class Password : ValueObject<string>
{
    //----------------------
    // Constants
    //----------------------
    public const int MinLength = 6;
    public const int MaxLength = 100;
    public const int HashMaxLength = 500;

    //----------------------
    // Properties
    //----------------------
    public string Hash { get; }

    //----------------------
    // Constructor (Private)
    //----------------------
    private Password(string hash):base(hash)
    {
        Hash = hash;
    }

    //----------------------
    // Factory Methods
    //----------------------

    /// <summary>
    /// Create from Plain Password (Register or Change Password)
    /// </summary>
    public static Result<Password> Create(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            return PasswordErrors.EmptyPassword();

        if (plainPassword.Length < MinLength)
            return PasswordErrors.TooShortPassword(MinLength);

        if (plainPassword.Length > MaxLength)
            return PasswordErrors.TooLongPassword(MaxLength);

        // Complexity validation
        var complexityResult = ValidateComplexity(plainPassword);
        if (complexityResult.IsFailure)
            return complexityResult.Error;

        // Hashing
        var hash = PasswordHasher.Hash(plainPassword);
        return Result.Success(new Password(hash));
    }

    /// <summary>
    /// Create from existing Hash (read from Database)
    /// </summary>
    public static Result<Password> CreateFromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return PasswordErrors.EmptyHash;

        return Result.Success(new Password(hash));
    }

    //----------------------
    // Behaviors
    //----------------------

    /// <summary>
    /// Password Verification
    /// </summary>
    public bool Verify(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            return false;

        return PasswordHasher.Verify(plainPassword, Hash);
    }

    //----------------------
    // Password Complexity Validation
    //----------------------
    private static Result ValidateComplexity(string password)
    {
        var hasUpperCase = password.Any(char.IsUpper);
        var hasLowerCase = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

        // At least 3 of the 4 conditions must be met
        var complexityScore = 0;
        if (hasUpperCase) complexityScore++;
        if (hasLowerCase) complexityScore++;
        if (hasDigit) complexityScore++;
        if (hasSpecialChar) complexityScore++;

        if (complexityScore < 3)
            return PasswordErrors.WeakComplexity;

        // Avoid common passwords
        var commonPasswords = new[]
        {
            "password", "123456", "12345678", "qwerty", "abc123",
            "password123", "admin", "letmein", "welcome"
        };

        if (commonPasswords.Any(cp => password.ToLowerInvariant().Contains(cp)))
            return PasswordErrors.TooCommon;

        // Prevent consecutive repeated characters (aaa, 111, ...)
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (password[i] == password[i + 1] && password[i] == password[i + 2])
                return PasswordErrors.RepeatingCharacters;
        }

        return Result.Success();
    }

    //----------------------
    // Equality
    //----------------------
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
    }
}

public static class PasswordErrors
{
    public static Error EmptyPassword() =>
        Error.Validation("Password.Empty", "رمز عبور نمی‌تواند خالی باشد");

    public static Error TooShortPassword(int minLength) =>
        Error.Validation("Password.TooShort", $"رمز عبور باید حداقل {minLength} کاراکتر باشد");

    public static Error TooLongPassword(int maxLength) =>
        Error.Validation("Password.TooLong", $"رمز عبور نباید بیشتر از {maxLength} کاراکتر باشد");

    public static Error WeakComplexity =>
        Error.Validation("Password.WeakComplexity",
            "رمز عبور باید حداقل شامل سه مورد از موارد زیر باشد: حروف بزرگ، حروف کوچک، اعداد، کاراکترهای خاص");

    public static Error TooCommon =>
        Error.Validation("Password.TooCommon", "رمز عبور انتخابی بسیار رایج است");

    public static Error RepeatingCharacters =>
        Error.Validation("Password.RepeatingCharacters",
            "رمز عبور نباید شامل کاراکترهای تکراری متوالی باشد");

    public static Error EmptyHash =>
        Error.Validation("Password.EmptyHash", "هش رمز عبور نمی‌تواند خالی باشد");
}
