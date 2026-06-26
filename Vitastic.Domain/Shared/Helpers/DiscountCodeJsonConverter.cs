using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Discounts.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class DiscountCodeJsonConverter(ILogger logger) : JsonConverter<DiscountCode>
    {
        public override void WriteJson(JsonWriter writer, DiscountCode? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override DiscountCode? ReadJson(JsonReader reader,
            Type objectType,
            DiscountCode? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
            => (DiscountCode)JsonConvertorExtensions.ReadJsonConvertorPart
            (reader, x=>DiscountCode.Create(x).Value,
                existingValue, logger, "کد تخفیف");
    }
}