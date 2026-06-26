namespace Vitastic.Domain.Shared.Exceptions;

/// <summary>
/// ForbiddenException is thrown when a user attempts to perform an operation
/// for which they do not have the required permissions.
/// </summary>
/// <param name="message">
/// The error message explaining why access to the requested operation is forbidden.
/// </param>
public class ForbiddenException(string message = "شما به این عملیات دسترسی ندارید")
    : DomainException("Access.Forbidden", message);
