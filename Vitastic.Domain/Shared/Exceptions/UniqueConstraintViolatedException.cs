namespace Vitastic.Domain.Shared.Exceptions;

/// <summary>public class UniqueConstraintViolatedException(string propertyName, object? propertyValue = null) : DomainException(
/// UniqueConstraintViolatedException is thrown when a unique constraint is violated in the domain.
/// </summary>
/// <param name="propertyName">The name of the property that violated the unique constraint.</param>
/// <param name="propertyValue">The value of the property that violated the unique constraint.</param>
public class UniqueConstraintViolatedException(string propertyName, object? propertyValue = null) : DomainException(
    "UniqueConstraint.Violated",
    $"مقدار '{propertyValue}' برای '{propertyName}' تکراری است و باید یکتا باشد.")
{

    public string PropertyName { get; } = propertyName;
    public object? PropertyValue { get; } = propertyValue;
}
