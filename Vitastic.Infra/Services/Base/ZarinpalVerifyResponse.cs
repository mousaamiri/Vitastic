using System.Text.Json.Serialization;

namespace Vitastic.Infra.Services.Base
{
    internal class ZarinpalVerifyResponse
    {
        public DataData? Data { get; set; }
        public string[]? Errors { get; set; }

        public class DataData
        {
            [JsonPropertyName("ref_Id")] public int RefId { get; set; }

            [JsonPropertyName("code")] public int Code { get; set; }

            [JsonPropertyName("message")] public string Message { get; set; }
        }
    }
}
