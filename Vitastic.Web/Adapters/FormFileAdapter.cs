using Vitastic.App.Common.Files;

namespace Vitastic.Web.Adapters;

public class FormFileAdapter(IFormFile fromFile):IFile
{
    public string FileName =>fromFile.FileName;
    public string ContentType => fromFile.ContentType;
    public long Length =>fromFile.Length;
    public Stream OpenReadStream()=> fromFile.OpenReadStream();
}