namespace Vitastic.Web.Models.ViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? ErrorMessage { get; set; }

        public bool ShowRequestId
        {
            get => !string.IsNullOrEmpty(RequestId);
            set => throw new NotImplementedException();
        }
    }
}
