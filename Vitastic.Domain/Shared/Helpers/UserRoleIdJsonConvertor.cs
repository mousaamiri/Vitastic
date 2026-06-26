using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class UserRoleIdJsonConvertor(ILogger logger) : JsonConverter<UserRoleId>
    {
        public override void WriteJson(JsonWriter writer, UserRoleId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override UserRoleId? ReadJson(JsonReader reader, Type objectType, UserRoleId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (UserRoleId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => UserRoleId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه نقش کاربر");
    }
}