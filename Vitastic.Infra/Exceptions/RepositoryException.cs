namespace Vitastic.Infra.Exceptions;

/// <summary>
/// RepositoryException is thrown when an error occurs in the repository layer of the application,
/// such as a failure to access the database or an error in data retrieval or manipulation.
/// </summary> <param name="code">The error code representing the specific repository error.</param>
/// <param name="message">A message describing the repository error.</param>
/// <param name="innerException">The inner exception that caused the repository error, if any.</param>
public class RepositoryException(string code,string message, Exception? innerException)
    : InfrastructureException(code,message, innerException);
