using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Wallets.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class WalletIdJsonConvertor(ILogger logger) : JsonConverter<WalletId>
    {
        public override void WriteJson(JsonWriter writer, WalletId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override WalletId? ReadJson(JsonReader reader, Type objectType, WalletId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (WalletId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => WalletId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه کیف پول");
    }
}