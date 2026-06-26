namespace Vitastic.Infra.Exceptions;
/// <summary>
///  InternalStorageError is thrown when an unexpected error occurs within the internal storage system,
/// such as a database failure or file system issue, that prevents the application from functioning correctly.
/// </summary>
/// <param name="message">The error message describing the internal storage error.</param>
public class InternalStorageError(string message) : Exception(message);
