using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class UserIdJsonConvertor(ILogger logger) : JsonConverter<UserId>
    {
        public override void WriteJson(JsonWriter writer, UserId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override UserId? ReadJson(JsonReader reader, Type objectType, UserId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (UserId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => UserId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه کاربر");
    }
}