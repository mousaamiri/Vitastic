using System.Reflection.PortableExecutable;

namespace Vitastic.Domain.Shared.Results;

public class Error(string code, string message,ErrorType errorType) : IEquatable<Error>
{
    public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.None);
    public static readonly Error NullValue = new Error("Error.NullValue", "مقدار null ارسال شده",ErrorType.Validation);

    public static Error NotFound(string code = "Error.NotFound", string message = "رکورد یافت نشد")
        => new(code, message, ErrorType.NotFound);
    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    public static Error Failure(string code, string message)
        => new(code, message, ErrorType.Failure);

    public static Error Unauthorized(string code = "Error.Unauthorized", string message = "عدم احراز هویت")
        => new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code = "Error.Forbidden", string message = "عدم دسترسی")
        => new(code, message, ErrorType.Forbidden);
    public static Error Verification(string code, string message="خطای اعتبار سنجی")
    =>new (code,message,ErrorType.Verification);
    public string Code => code;
    public string Message => message;
    public ErrorType ErrorType => errorType;
    public static bool operator ==(Error? x, Error? y)
    {
        if (x is null && y is null)
            return true;
        if (x is null || y is null)
            return false;
        return x.Equals(y);
    }

    public static bool operator !=(Error? x, Error? y) => !(x == y);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals((Error)obj);
    }

    public override int GetHashCode() => HashCode.Combine(code, message);
    public override string ToString() => $"{code}:{message}";
    public bool Equals(Error? other)
    {
        if (other is null)
            return false;

        return string.Equals(code, other.Code, StringComparison.Ordinal) && string.Equals(message, other.Message, StringComparison.Ordinal) && errorType == other.ErrorType;
    }


}
