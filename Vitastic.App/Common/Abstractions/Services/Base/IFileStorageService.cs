using Vitastic.App.Common.Abstractions.Files;

namespace Vitastic.App.Common.Abstractions.Services.Base
{
    /// <summary>
    /// Defines the contract for a file storage service that handles uploading, deleting, and retrieving files
    /// such as images, thumbnails, and videos. Files are organized in entity-specific directories.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file to the appropriate entity directory and returns the generated file name.
        /// The file is stored based on the entity type, entity ID, and file type.
        /// </summary>
        /// <param name="file">The file to be uploaded (implementing IFile interface).</param>
        /// <param name="entityType">The type of the entity (e.g., "Course", "User"). Uses nameof() for consistency.</param>
        /// <param name="entityId">The ID of the entity to which the file belongs.</param>
        /// <param name="fileType">The type of the file (e.g., Image, Thumbnail, Video).</param>
        /// <returns>The generated file name (without path) under which the file is stored. This is the name that should be saved in the database.</returns>
        Task<string> UploadFileAsync(IFile file, string entityType, Guid entityId, FileType fileType);

        /// <summary>
        /// Uploads a file and generates its thumbnail simultaneously.
        /// Returns both the main file name and thumbnail file name.
        /// </summary>
        /// <param name="file">The main file to upload</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="entityId">The entity ID</param>
        /// <param name="fileType">The file type (e.g., Image)</param>
        /// <param name="thumbnailOptions">Options for thumbnail generation (size, quality, etc.)</param>
        /// <returns>A tuple containing main file name and thumbnail file name</returns>
        Task<(string MainFileName, string ThumbnailFileName)> UploadFileWithThumbnailAsync(
            IFile file,
            string entityType,
            Guid entityId,
            ThumbnailOptions? thumbnailOptions = null);

        /// <summary>
        /// Deletes a file associated with a specific entity and file type.
        /// </summary>
        /// <param name="entityType">The type of the entity (e.g., "Course", "User").</param>
        /// <param name="entityId">The ID of the entity to which the file belongs.</param>
        /// <param name="fileType">The type of the file (e.g., Image, Thumbnail, Video).</param>
        /// <param name="fileName">The name of the file to delete (without path). This should match the name stored in the database.</param>
        /// <returns>True if the file was successfully deleted; otherwise, false.</returns>
        Task<bool> DeleteFileAsync(string entityType, Guid entityId, FileType fileType, string fileName);

        /// <summary>
        /// Checks if a file exists in the storage.
        /// </summary>
        /// <param name="entityType">The type of the entity (e.g., "Course", "User").</param>
        /// <param name="entityId">The ID of the entity to which the file belongs.</param>
        /// <param name="fileType">The type of the file (e.g., Image, Thumbnail, Video).</param>
        /// <param name="fileName">The name of the file (without path).</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        bool FileExists(string entityType, Guid entityId, FileType fileType, string fileName);

        /// <summary>
        /// Retrieves the full path (including directory structure) of a stored file.
        /// </summary>
        /// <param name="entityType">The type of the entity (e.g., "Course", "User").</param>
        /// <param name="entityId">The ID of the entity to which the file belongs.</param>
        /// <param name="fileType">The type of the file (e.g., Image, Thumbnail, Video).</param>
        /// <param name="fileName">The name of the file (without path). This should match the name stored in the database.</param>
        /// <returns>The full physical or virtual path to the file.</returns>
        string GetFilePath(string entityType, Guid entityId, FileType fileType, string fileName);

        /// <summary>
        /// Retrieves only the file name (without path) for a stored file. Useful for references in databases.
        /// This method typically returns the same fileName passed as input, but can be used for validation.
        /// </summary>
        /// <param name="entityType">The type of the entity (e.g., "Course", "User").</param>
        /// <param name="entityId">The ID of the entity to which the file belongs.</param>
        /// <param name="fileType">The type of the file (e.g., Image, Thumbnail, Video).</param>
        /// <param name="fileName">The name of the file (without path).</param>
        /// <returns>The file name (without path).</returns>
        string GetFileName(string entityType, Guid entityId, FileType fileType, string fileName);
    }

    /// <summary>
    /// Configuration options for thumbnail generation
    /// </summary>
    public class ThumbnailOptions
    {
        public int MaxWidth { get; set; } = 300;
        public int MaxHeight { get; set; } = 300;
        public int Quality { get; set; } = 80;
        public string Format { get; set; } = "jpg";
    }

    /// <summary>
    /// Represents the type of file being stored (e.g., Image, Thumbnail, Video).
    /// This helps in organizing files into appropriate directories.
    /// </summary>
    public enum FileType
    {
        Image,
        Thumbnail,
        Video
    }
}
