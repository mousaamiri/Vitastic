namespace Vitastic.Domain.Shared.Exceptions;

/// <summary>
/// ValidationException is thrown when a validation error occurs in the domain.
/// </summary>
/// <param name="entityName">The name of the entity that failed validation.</param>
/// <param name="propertyName">The name of the property that failed validation.</param>
/// <param name="message">The error message explaining the validation failure.</param>
public class ValidationException(string entityName, string propertyName, string message)
    : DomainException("Validation.Error", $"{entityName} با {propertyName} معتبر نیست. {message}");
