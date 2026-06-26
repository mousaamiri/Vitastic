using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Users.ValueObjects;

public sealed class ActiveCode:ValueObject<string>
{
    //Object to generate code
    public string Code { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }

    public static int MaxLength = 32; // GUID string length without dashes

    //Default expiration time for the code is 24 hours
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(24);
    //Constructor to generate code
    private ActiveCode(string code, DateTimeOffset expiresAt):base(code)
    {
        Code = code;
        ExpiresAt = expiresAt;
    }
    /// <summary>
    ///  Generates a new active code with a random string
    /// and an expiration time of 24 hours from now.
    /// </summary>
    /// <returns> A new instance of ActiveCode</returns>
    public static ActiveCode Generate()
    {
        return new ActiveCode(Guid.NewGuid().ToString("N")
            , DateTime.UtcNow.Add(DefaultExpiration));
    }
    /// <summary>
    /// Regenerates a new active code from database
    /// </summary>
    /// <param name="code">Database code</param>
    /// <param name="expiresAt">Expiration time from database</param>
    /// <returns> A Result containing a new instance of ActiveCode if successful, or an error otherwise</returns>
    public static Result<ActiveCode> Create(string code, DateTimeOffset? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(code))
            return ActiveCodeErrors.CodeEmpty();
        if(!Guid.TryParse(code, out _))
            return ActiveCodeErrors.InvalidFormat();
        if (expiresAt.HasValue && expiresAt <= DateTimeOffset.UtcNow)
            return ActiveCodeErrors.ExpiresAtInvalid();
        return Result.Success(new ActiveCode(code, expiresAt?? DateTimeOffset.UtcNow));
    }
    //Check if the code is still valid (not expired)
    public bool IsValid()=>DateTime.UtcNow>=ExpiresAt;
    //Check if the code is expired
    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    //Get the remaining time until the code expires
    public TimeSpan TimeUntilExpiration() => ExpiresAt - DateTime.UtcNow;
    //--------------------
    //Override
    //--------------------
    //Get equality components for value object comparison
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
    //ToString
    public override string ToString() => $"Code: {Code}, ExpiresAt: {ExpiresAt}";

}
public static class ActiveCodeErrors
{
    public static Error CodeEmpty()
        =>  Error.Validation("ActiveCode.CodeEmpty", "کد نمی‌تواند خالی باشد.");
    public static Error ExpiresAtInvalid()
        =>  Error.Validation("ActiveCode.ExpiresAtInvalid", "زمان انقضا باید در آینده باشد.");
    public static Error InvalidFormat()
        =>  Error.Validation("ActiveCode.InvalidFormat", "کد باید یک GUID معتبر باشد.");

}
