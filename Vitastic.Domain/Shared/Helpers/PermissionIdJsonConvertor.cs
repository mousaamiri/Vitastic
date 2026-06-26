using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Roles.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class PermissionIdJsonConvertor(ILogger logger) : JsonConverter<PermissionId>
    {
        public override void WriteJson(JsonWriter writer, PermissionId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override PermissionId? ReadJson(JsonReader reader, Type objectType, PermissionId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (PermissionId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => PermissionId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه دسترسی");
    }
}