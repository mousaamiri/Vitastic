namespace Vitastic.Domain.Shared.Exceptions;
/// <summary>
///  DuplicateEntityException is thrown when an attempt
/// is made to create or update an entity with a property value that must be unique.
/// </summary>
/// <param name="entityName"> Name of the entity </param>
/// <param name="propertyName"> Name of the property </param>
/// <param name="value"> Value of the property </param>
public class DuplicateEntityException
    (string entityName, string propertyName, object value)
    : DomainException("Entity.Duplicate",
        $"{entityName} با {propertyName} = '{value}' قبلاً ثبت شده است");
