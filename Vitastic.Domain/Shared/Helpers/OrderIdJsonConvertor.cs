using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Orders.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class OrderIdJsonConvertor(ILogger logger) : JsonConverter<OrderId>
    {
        public override void WriteJson(JsonWriter writer, OrderId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override OrderId? ReadJson(JsonReader reader, Type objectType, OrderId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (OrderId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => OrderId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه سفارش");
    }
}