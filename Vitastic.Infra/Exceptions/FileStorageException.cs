namespace Vitastic.Infra.Exceptions;

/// <summary>
/// Thrown when a file storage operation (upload, download, delete, etc.) fails.
/// </summary>
/// <param name="operation">The failed operation name.</param>
/// <param name="message">Error description.</param>
/// <param name="fileKey">Optional file identifier.</param>
/// <param name="innerException">Optional underlying exception.</param>
public sealed class FileStorageException(
    string operation,
    string message,
    string? fileKey = null,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    /// <summary>
    /// The name of the failed operation.
    /// </summary>
    public string Operation { get; } = operation;

    /// <summary>
    /// The related file identifier (if any).
    /// </summary>
    public string? FileKey { get; } = fileKey;

    /// <summary>
    /// Creates an exception for failed upload operations.
    /// </summary>
    public static FileStorageException UploadFailed(
        string fileName,
        Exception innerException) =>
        new("Upload", $"آپلود فایل '{fileName}' ناموفق بود", fileName, innerException);

    /// <summary>
    /// Creates an exception for failed delete operations.
    /// </summary>
    public static FileStorageException DeleteFailed(
        string fileKey,
        Exception innerException) =>
        new("Delete", $"حذف فایل '{fileKey}' ناموفق بود", fileKey, innerException);

    /// <summary>
    /// Creates an exception for failed download operations.
    /// </summary>
    public static FileStorageException DownloadFailed(
        string fileKey,
        Exception innerException) =>
        new("Download", $"دانلود فایل '{fileKey}' ناموفق بود", fileKey, innerException);

    /// <summary>
    /// Creates an exception for missing configuration.
    /// </summary>
    public static FileStorageException ConfigurationMissing(string key) =>
        new("Configuration", $"تنظیمات '{key}' یافت نشد");
}
