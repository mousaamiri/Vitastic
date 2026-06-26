using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Instructors.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class InstructorIdJsonConvertor(ILogger logger) : JsonConverter<InstructorId>
    {
        public override void WriteJson(JsonWriter writer, InstructorId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override InstructorId? ReadJson(JsonReader reader, Type objectType, InstructorId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (InstructorId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => InstructorId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه مدرس");
    }
}