using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Orders.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class OrderItemIdJsonConvertor(ILogger logger) : JsonConverter<OrderItemId>
    {
        public override void WriteJson(JsonWriter writer, OrderItemId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override OrderItemId? ReadJson(JsonReader reader, Type objectType, OrderItemId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (OrderItemId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => OrderItemId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه آیتم سفارش");
    }
}