using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class FullNameJsonConverter(ILogger logger) : JsonConverter<FullName>
    {
        public override void WriteJson(JsonWriter writer, FullName? value, JsonSerializer serializer)
            => JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override FullName? ReadJson(JsonReader reader, Type objectType, FullName? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
            => (FullName)JsonConvertorExtensions.ReadJsonConvertorPart
            (reader, x=>FullName.Create(x).Value,
                existingValue, logger, "نام کامل");
    }
}