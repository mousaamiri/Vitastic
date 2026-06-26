using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.Helpers
{
    public static class JsonConvertorExtensions
    {
        public static void WriteJsonConvertorPart<TValueType>(JsonWriter writer, ValueObject<TValueType>? value)
        {
            if (value is null || value.Value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("Value");
            writer.WriteValue(value.Value);
            writer.WriteEndObject();
        }
        public static ValueObject<TValueType> ReadJsonConvertorPart<TValueType>
        (JsonReader reader
            , Func<string, Result<ValueObject<TValueType>>> factoryResult
            , ValueObject<TValueType>? value
            , ILogger logger
            , string perseanName)
        {
            if (reader.TokenType is JsonToken.Null)
                return null;

            string? valueObjectString = null;
            if (reader.TokenType is JsonToken.StartObject)
            {
                var jobject = JObject.Load(reader);
                valueObjectString = jobject["Value"]?.ToString();
            }
            else if (reader.TokenType is JsonToken.String)
            {
                valueObjectString = reader.Value?.ToString();
            }

            if (string.IsNullOrEmpty(valueObjectString))
                return null;

            Result<ValueObject<TValueType>> result = factoryResult(valueObjectString!);
            if (result.IsFailure)
            {
                logger.LogInformation(
                    $"{perseanName} نامعتبر است: {valueObjectString}. خطا: {result.Error.Code} - {result.Error.Message}",
                    $"{value.GetType().Name}JsonConverter.{value.GetType().Name}.Create.Invalid", DateTime.UtcNow,
                    result.Error, result.Error);
                throw new JsonSerializationException($"{perseanName} نامعتبر است: {result.Error}");
            }

            return result.Value;
        }
    }
}