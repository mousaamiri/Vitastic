using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Categories.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class CategoryIdJsonConvertor(ILogger logger) : JsonConverter<CategoryId>
    {
        public override void WriteJson(JsonWriter writer, CategoryId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override CategoryId? ReadJson(JsonReader reader, Type objectType, CategoryId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (CategoryId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => CategoryId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه دسته");
    }
}