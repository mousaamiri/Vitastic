namespace Vitastic.Domain.Shared.Exceptions;
/// <summary>
/// BusinessRuleViolationException is thrown when a specific business rule is violated within the domain logic.
/// </summary>
/// <param name="errorCode">The error code representing the specific business rule violation.</param>
/// <param name="message">A message describing the business rule violation.</param>
public class BusinessRuleViolationException(string errorCode, string message)
    : DomainException(errorCode, message);
