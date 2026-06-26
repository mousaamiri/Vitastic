using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Tags.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class TagIdJsonConvertor(ILogger logger) : JsonConverter<TagId>
    {
        public override void WriteJson(JsonWriter writer, TagId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override TagId? ReadJson(JsonReader reader, Type objectType, TagId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (TagId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => TagId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه برچسب");
    }
}