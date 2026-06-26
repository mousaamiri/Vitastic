using System.Text.Json.Serialization;

namespace Vitastic.Infra.Services.Base
{
    public class ZarinpalCreateResponse
    {
        public DataData? Data { get; set; }
        public string[]? Errors { get; set; }

        public class DataData
        {
            [JsonPropertyName("authority")] public string Authority { get; set; }

            [JsonPropertyName("code")] public int Code { get; set; }

            [JsonPropertyName("message")] public string Message { get; set; }
        }
    }
}
