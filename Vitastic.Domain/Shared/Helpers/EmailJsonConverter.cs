using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers;

public sealed class EmailJsonConverter(ILogger logger) : JsonConverter<Email>
{
    public override void WriteJson(JsonWriter writer, Email? value, JsonSerializer serializer)
        => JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

    public override Email? ReadJson(JsonReader reader, Type objectType, Email? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
        => (Email)JsonConvertorExtensions.ReadJsonConvertorPart
           (reader, x=>Email.Create(x).Value,
               existingValue, logger, "ایمیل");
}
