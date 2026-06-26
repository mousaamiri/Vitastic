using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class MoneyJsonConverter(ILogger logger) : JsonConverter<Money>
    {
        public override void WriteJson(JsonWriter writer, Money? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override Money? ReadJson(JsonReader reader,
            Type objectType,
            Money? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
            => (Money)JsonConvertorExtensions.ReadJsonConvertorPart
            (reader, x=>Money.Create(decimal.Parse(x)).Value,
                existingValue, logger, "پول");
    }
}