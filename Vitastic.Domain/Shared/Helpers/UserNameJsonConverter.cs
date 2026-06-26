using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class UserNameJsonConverter(ILogger logger) : JsonConverter<UserName>
    {
        public override void WriteJson(JsonWriter writer, UserName? value, JsonSerializer serializer)
            => JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override UserName? ReadJson(JsonReader reader, Type objectType, UserName? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
            => (UserName)JsonConvertorExtensions.ReadJsonConvertorPart
            (reader, x=>UserName.Create(x).Value,
                existingValue, logger, "نام کاربری");
    }
}