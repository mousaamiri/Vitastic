using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Infra.Exceptions;
using Vitastic.Infra.Settings;

namespace Vitastic.Infra.Services.Base;

public sealed class BackblazeStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly BackblazeSettings _settings;
    private readonly ILogger<BackblazeStorageService> _logger;

    public BackblazeStorageService(
        IOptions<BackblazeSettings> settings,
        ILogger<BackblazeStorageService> logger)
    {
        _settings = settings.Value ?? throw FileStorageException.ConfigurationMissing("BackblazeSettings");
        _logger = logger;

        try
        {
            var credentials = new BasicAWSCredentials(
                _settings.KeyId,
                _settings.ApplicationKey
            );

            var config = new AmazonS3Config
            {
                ServiceURL = _settings.Endpoint,
                ForcePathStyle = true
            };

            _s3Client = new AmazonS3Client(credentials, config);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize Backblaze S3 client");
            throw new FileStorageException(
                "Initialization",
                "ایجاد ارتباط با سرویس ذخیره‌سازی ناموفق بود",
                innerException: ex);
        }
    }

    public Task<(string ImageKey, string ThumbnailKey)> UploadImageWithThumbnailAsync(Stream fileStream, string fileName, string folder, int thumbnailMaxSize = 300,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateThumbnailAsync(string existingImageKey, int maxSize = 300, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<string> UploadImageAsync(
        Stream fileStream, string fileName, string folder,
        CancellationToken cancellationToken = default)
    {
        // Generate unique key
        var fileKey = $"{folder}/{Guid.NewGuid():N}{Path.GetExtension(fileName)}";

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = fileKey,
                InputStream = fileStream,
                ContentType = GetContentType(fileName),
                CannedACL = S3CannedACL.Private
            };

            _logger.LogInformation(
                "Uploading image {FileName} to {FileKey}",
                fileName,
                fileKey);

            var response = await _s3Client.PutObjectAsync(request, cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError(
                    "Upload failed with status code {StatusCode}",
                    response.HttpStatusCode);

                throw new FileStorageException(
                    "Upload",
                    $"آپلود با کد خطای {response.HttpStatusCode} ناموفق بود",
                    fileKey);
            }

            _logger.LogInformation("Successfully uploaded image to {FileKey}", fileKey);
            return fileKey;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error while uploading {FileName}", fileName);
            throw FileStorageException.UploadFailed(fileName, ex);
        }
        catch (Exception ex) when (ex is not FileStorageException)
        {
            _logger.LogError(ex, "Unexpected error while uploading {FileName}", fileName);
            throw FileStorageException.UploadFailed(fileName, ex);
        }
    }

    public async Task DeleteImageAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = publicId
            };

            _logger.LogInformation("Deleting image {FileKey}", publicId);

            await _s3Client.DeleteObjectAsync(request, cancellationToken);

            _logger.LogInformation("Successfully deleted image {FileKey}", publicId);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error while deleting {FileKey}", publicId);
            throw FileStorageException.DeleteFailed(publicId, ex);
        }
        catch (Exception ex) when (ex is not FileStorageException)
        {
            _logger.LogError(ex, "Unexpected error while deleting {FileKey}", publicId);
            throw FileStorageException.DeleteFailed(publicId, ex);
        }
    }

    public string GetImageUrl(
        string publicId,
        ImageTransformation? transformation = null)
    {
        // For Backblaze, we'd need signed URLs or CDN
        // This is a simple implementation
        return $"{_settings.Endpoint}/{_settings.BucketName}/{publicId}";
    }

    public async Task<string> UploadVideoAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken cancellationToken = default)
    {
        // Same as UploadImageAsync but for videos
        return await UploadImageAsync(fileStream, fileName, folder, cancellationToken);
    }

    public async Task DeleteVideoAsync(
        string fileKey,
        CancellationToken cancellationToken = default)
    {
        await DeleteImageAsync(fileKey, cancellationToken);
    }

    public async Task<string> GetVideoSignedUrlAsync(
        string fileKey,
        TimeSpan expiresIn,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = fileKey,
                Expires = DateTime.UtcNow.Add(expiresIn),
                Protocol = Protocol.HTTPS,
                Verb = HttpVerb.GET
            };

            _logger.LogInformation(
                "Generating signed URL for {FileKey} (expires in {Minutes} minutes)",
                fileKey,
                expiresIn.TotalMinutes);

            var url = await _s3Client.GetPreSignedURLAsync(request);

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating signed URL for {FileKey}", fileKey);
            throw new FileStorageException(
                "GenerateSignedUrl",
                $"تولید لینک موقت برای '{fileKey}' ناموفق بود",
                fileKey,
                ex);
        }
    }

    private static string GetContentType(string fileName) =>
        Path.GetExtension(fileName).ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".mp4" => "video/mp4",
            ".mkv" => "video/x-matroska",
            ".avi" => "video/avi",
            ".mov" => "video/quicktime",
            ".webm" => "video/webm",
            _ => "application/octet-stream"
        };
}
