using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class PhoneNumberJsonConverter(ILogger logger) : JsonConverter<PhoneNumber>
    {
        public override void WriteJson(JsonWriter writer, PhoneNumber? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override PhoneNumber? ReadJson(JsonReader reader,
            Type objectType,
            PhoneNumber? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
            => (PhoneNumber)JsonConvertorExtensions.ReadJsonConvertorPart
            (reader, x=>PhoneNumber.Create(x).Value,
                existingValue, logger, "شماره تلفن");
    }
}