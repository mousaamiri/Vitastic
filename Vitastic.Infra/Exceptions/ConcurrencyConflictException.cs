namespace Vitastic.Infra.Exceptions;

/// <summary>
/// ConcurrencyConflictException is thrown when a concurrency conflict occurs during database operations,
/// such as when multiple users attempt to edit the same entity simultaneously, resulting in a conflict that prevents the operation from completing successfully.
/// </summary> <param name="entityName">The name of the entity that is involved in the concurrency conflict.</param>
///  <param name="innerException">The inner exception that caused the concurrency conflict, providing additional details about the error.</param>
public class ConcurrencyConflictException(string entityName, Exception innerException)
    : InfrastructureException(
        "Database.ConcurrencyConflict", $"تداخل در ویرایش همزمان {entityName}", innerException);
