namespace Vitastic.App.Common.Abstractions.Services.Base;
public interface IFileUrlService
{
    string GetFileUrl(string entityType, Guid entityId, FileType fileType, string fileName);
    string GetBaseApiUrl();
}
