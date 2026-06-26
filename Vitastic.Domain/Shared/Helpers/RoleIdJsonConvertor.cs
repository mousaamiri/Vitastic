using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Roles.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class RoleIdJsonConvertor(ILogger logger) : JsonConverter<RoleId>
    {
        public override void WriteJson(JsonWriter writer, RoleId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override RoleId? ReadJson(JsonReader reader, Type objectType, RoleId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (RoleId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => RoleId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه نقش");
    }
}