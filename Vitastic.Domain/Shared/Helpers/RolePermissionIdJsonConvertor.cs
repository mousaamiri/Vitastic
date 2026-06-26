using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Roles.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class RolePermissionIdJsonConvertor(ILogger logger) : JsonConverter<RolePermissionId>
    {
        public override void WriteJson(JsonWriter writer, RolePermissionId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override RolePermissionId? ReadJson(JsonReader reader, Type objectType,
            RolePermissionId? existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            (RolePermissionId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => RolePermissionId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه دسترسی نقش");
    }
}