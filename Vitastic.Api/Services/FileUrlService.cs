using Microsoft.AspNetCore.Server.IISIntegration;
using Vitastic.App.Common.Abstractions.Services.Base;

namespace Vitastic.Api.Services;

public class FileUrlService(IHttpContextAccessor httpContextAccessor) : IFileUrlService
{
    public string GetFileUrl(string entityType, Guid entityId, FileType fileType, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return string.Empty;
        HttpRequest? request = httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        if (IsDefault(fileName))
        {
            return $"{baseUrl}/storage/{entityType}/Default/{fileName}";
        }


        return $"{baseUrl}/storage/{entityType}/{entityId}/{fileType}/{fileName}";
    }

    private bool IsDefault(string fileName) => fileName.StartsWith("default");

    public string GetBaseApiUrl()
    {
        HttpRequest? request = httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        return baseUrl;
    }
}
