namespace Vitastic.App.Common.Abstractions.Files;

public interface IFile
{
    string FileName { get; set; }
    string ContentType { get; set; }
    long Length { get; set; }
    Stream OpenReadStream();
}
