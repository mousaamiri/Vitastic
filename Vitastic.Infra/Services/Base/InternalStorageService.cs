using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Services.Base;
using Microsoft.Extensions.Logging;

namespace Vitastic.Infra.Services.Base
{
    public class InternalStorageService : IFileStorageService
    {
        private readonly string _baseStoragePath;
        private readonly ILogger<InternalStorageService> _logger;

        public InternalStorageService(ILogger<InternalStorageService> logger)
        {
            _logger = logger;
            _baseStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "storage");

            // Ensure base directory exists
            if (!Directory.Exists(_baseStoragePath))
            {
                Directory.CreateDirectory(_baseStoragePath);
                _logger.LogInformation("Created base storage directory: {DirectoryPath}", _baseStoragePath);
            }
        }

        public async Task<string> UploadFileAsync(IFile file, string entityType, Guid entityId, FileType fileType)
        {
            _logger.LogInformation("Uploading file for entity: {EntityType} {EntityId} {FileType}",
                entityType, entityId, fileType);

            try
            {
                var fileName = GenerateFileName(file.FileName);
                var filePath = GetFullFilePath(entityType, entityId, fileType, fileName);

                EnsureDirectoryExists(filePath);

                await using FileStream fileStream = File.Create(filePath);
                await file.OpenReadStream().CopyToAsync(fileStream);

                _logger.LogInformation("File uploaded successfully: {FileName} -> {FilePath}", fileName, filePath);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file for entity: {EntityType} {EntityId} {FileType}",
                    entityType, entityId, fileType);
                throw;
            }
        }

        public async Task<(string MainFileName, string ThumbnailFileName)> UploadFileWithThumbnailAsync(
            IFile file, string entityType, Guid entityId, ThumbnailOptions? thumbnailOptions = null)
        {
            FileType fileType = FileType.Image;
            _logger.LogInformation("Uploading file with thumbnail for entity: {EntityType} {EntityId} {Type}",
                entityType, entityId, fileType);

            thumbnailOptions ??= new ThumbnailOptions();

            try
            {
                // Upload main file
                var mainFileName = await UploadFileAsync(file, entityType, entityId, fileType);

                // Generate and upload thumbnail
                var thumbnailFileName = await GenerateAndSaveThumbnailAsync(file, entityType, entityId, thumbnailOptions);

                _logger.LogInformation("File and thumbnail uploaded successfully: Main={MainFile}, Thumbnail={Thumbnail}",
                    mainFileName, thumbnailFileName);

                return (mainFileName, thumbnailFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file with thumbnail for entity: {EntityType} {EntityId} {FileType}",
                    entityType, entityId, fileType);
                throw;
            }
        }

        public Task<bool> DeleteFileAsync(string entityType, Guid entityId, FileType fileType, string fileName)
        {
            _logger.LogInformation("Deleting file: {EntityType} {EntityId} {FileType} {FileName}",
                entityType, entityId, fileType, fileName);

            try
            {
                var filePath = GetFullFilePath(entityType, entityId, fileType, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return Task.FromResult(true);
                }

                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {EntityType} {EntityId} {FileType} {FileName}",
                    entityType, entityId, fileType, fileName);
                throw;
            }
        }

        public bool FileExists(string entityType, Guid entityId, FileType fileType, string fileName)
        {
            var filePath = GetFullFilePath(entityType, entityId, fileType, fileName);
            bool exists = File.Exists(filePath);

            _logger.LogDebug("File existence check: {FilePath} -> {Exists}", filePath, exists);
            return exists;
        }

        public string GetFilePath(string entityType, Guid entityId, FileType fileType, string fileName)
        {
            var path = GetFullFilePath(entityType, entityId, fileType, fileName);
            _logger.LogDebug("Generated file path: {Path}", path);
            return path;
        }

        public string GetFileName(string entityType, Guid entityId, FileType fileType, string fileName)
        {
            _logger.LogDebug("Returning file name: {FileName}", fileName);
            return fileName;
        }

        #region Private Methods

        private string GenerateFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            _logger.LogDebug("Generated file name: {FileName} from original: {OriginalFileName}",
                fileName, originalFileName);

            return fileName;
        }

        private string GetFullFilePath(string entityType, Guid entityId, FileType fileType, string fileName)
        {
            return Path.Combine(_baseStoragePath, entityType, entityId.ToString(), fileType.ToString(), fileName);
        }

        private void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation("Created directory: {Directory}", directory);
            }
        }

        private async Task<string> GenerateAndSaveThumbnailAsync(
            IFile file, string entityType, Guid entityId, ThumbnailOptions options)
        {
            _logger.LogInformation("Generating thumbnail for entity: {EntityType} {EntityId}", entityType, entityId);

            try
            {
                await using Stream stream = file.OpenReadStream();

                // Load image using ImageSharp
                using Bitmap originalBitmap = new Bitmap(stream);

                _logger.LogDebug("Original image dimensions: {Width}x{Height}", originalBitmap.Width, originalBitmap.Height);

                var (newWidth, newHeight) = CalculateDimensions(
                    originalBitmap.Width, originalBitmap.Height, options.MaxWidth, options.MaxHeight);

                _logger.LogDebug("Thumbnail dimensions: {Width}x{Height}", newWidth, newHeight);

                // Resize image with high quality resampler
                using Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);
                using Graphics graphics = Graphics.FromImage(resizedBitmap);
                // High quality resize settings
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);

                // Generate thumbnail file name
                var thumbnailFileName = $"{Guid.NewGuid()}.{options.Format.ToLower()}";
                var thumbnailPath = GetFullFilePath(entityType, entityId, FileType.Thumbnail, thumbnailFileName);

                EnsureDirectoryExists(thumbnailPath);

                // Get encoder based on format
                var(imageFormat, encoderParams)  = GetImageEncoder(options.Format, options.Quality);

                // Save thumbnail
                await Task.Run(() =>
                {
                    using FileStream outputStream = File.Create(thumbnailPath);
                    resizedBitmap.Save(outputStream, imageFormat);
                });

                _logger.LogInformation("Thumbnail generated successfully: {FileName} -> {FilePath}",
                    thumbnailFileName, thumbnailPath);

                return thumbnailFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate thumbnail for entity: {EntityType} {EntityId}",
                    entityType, entityId);
                throw new InvalidOperationException("Failed to generate thumbnail", ex);
            }
        }

        private (int Width, int Height) CalculateDimensions(int originalWidth, int originalHeight, int maxWidth,
            int maxHeight)
        {
            // Calculate ratio while maintaining aspect ratio
            var ratioX = (double)maxWidth / originalWidth;
            var ratioY = (double)maxHeight / originalHeight;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(originalWidth * ratio);
            var newHeight = (int)(originalHeight * ratio);

            // Ensure minimum dimensions of 1 pixel
            return (Math.Max(1, newWidth), Math.Max(1, newHeight));
        }

        private (ImageFormat Format, EncoderParameters? Params) GetImageEncoder(string format, int quality)
        {
            ImageFormat imageFormat = format.ToLower() switch
            {
                "jpg" or "jpeg" => ImageFormat.Jpeg,
                "png" => ImageFormat.Png,
                "gif" => ImageFormat.Gif,
                "bmp" => ImageFormat.Bmp,
                _ => ImageFormat.Jpeg
            };

            _logger.LogDebug("Selected format: {Format} with quality: {Quality}", imageFormat, quality);
            return (imageFormat, null);
        }

        #endregion
    }
}
