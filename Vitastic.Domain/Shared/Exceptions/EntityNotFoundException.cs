namespace Vitastic.Domain.Shared.Exceptions;
/// <summary>
///  EntityNotFoundException is thrown when a requested entity cannot be found in the domain.
/// </summary>
/// <param name="entityName">The name of the entity that was not found.</param>
/// <param name="entityId">The identifier of the entity that was not found.</param
public class EntityNotFoundException(string entityName, object entityId)
    : DomainException("Entity.NotFound", $"{entityName} با شناسه {entityId} یافت نشد");
