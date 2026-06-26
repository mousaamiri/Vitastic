namespace Vitastic.Domain.Shared.Exceptions;
/// <summary>
/// Base class for all domain exceptions.
/// It includes an error code to identify the specific error type,
/// and a message describing the error.
/// This allows for consistent error handling across the domain layer,
/// and provides a way to communicate specific error information to the
/// application layer or API layer without exposing internal details of the domain logic.
/// </summary>
public abstract class DomainException: Exception
{
    public string ErrorCode { get; }

    protected DomainException(string errorCode, string message)
        : base(message) => ErrorCode = errorCode;

    protected DomainException(string errorCode, string message, Exception innerException)
        : base(message, innerException) => ErrorCode = errorCode;
}
