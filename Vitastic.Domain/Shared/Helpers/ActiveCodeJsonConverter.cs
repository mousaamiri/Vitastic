using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class ActiveCodeJsonConverter(ILogger logger) : JsonConverter<ActiveCode>
    {
        public override void WriteJson(JsonWriter writer, ActiveCode? value, JsonSerializer serializer)
            => JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override ActiveCode? ReadJson(JsonReader reader, Type objectType, ActiveCode? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
            => (ActiveCode)JsonConvertorExtensions.ReadJsonConvertorPart
            (reader, x=>ActiveCode.Create(x,null).Value,
                existingValue, logger, "کد فعالسازی کاربر");
    }
}