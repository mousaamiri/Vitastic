using Vitastic.App.Common.Abstractions.Files;

namespace Vitastic.Api.Wrapper
{
    public class FormFileAdapter(IFormFile formFile) : IFile
    {
        public string FileName
        {
            get => formFile.FileName;
            set { }
        }

        public string ContentType
        {
            get => formFile.ContentType;
            set { }
        }

        public long Length
        {
            get => formFile.Length;
            set { }
        }

        public Stream OpenReadStream() => formFile.OpenReadStream();
    }
}
