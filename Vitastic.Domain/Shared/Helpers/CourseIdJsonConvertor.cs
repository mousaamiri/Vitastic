using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vitastic.Domain.Entities.Courses.ValueObjects;

namespace Vitastic.Domain.Shared.Helpers
{
    public sealed class CourseIdJsonConvertor(ILogger logger) : JsonConverter<CourseId>
    {
        public override void WriteJson(JsonWriter writer, CourseId? value, JsonSerializer serializer) =>
            JsonConvertorExtensions.WriteJsonConvertorPart(writer, value);

        public override CourseId? ReadJson(JsonReader reader, Type objectType, CourseId? existingValue,
            bool hasExistingValue, JsonSerializer serializer) =>
            (CourseId)JsonConvertorExtensions.ReadJsonConvertorPart(
                reader,
                x => CourseId.CreateFrom(x).Value,
                existingValue,
                logger,
                "شناسه دوره");
    }
}