using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Transactions.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class PaymentTransactionIdJsonConvertor(ILogger logger) : JsonConverter<PaymentTransactionId>
    {
        public override void WriteJson(JsonWriter writer, PaymentTransactionId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override PaymentTransactionId? ReadJson(JsonReader reader, Type objectType,
            PaymentTransactionId? existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            (PaymentTransactionId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => PaymentTransactionId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه تراکنش پرداخت");
    }
}