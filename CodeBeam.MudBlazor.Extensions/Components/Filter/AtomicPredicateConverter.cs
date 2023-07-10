using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudExtensions
{
#nullable enable
    public class AtomicPredicateConverter<T> : JsonConverter<AtomicPredicate<T>>
    {
        public override AtomicPredicate<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var atomicPredicate = new AtomicPredicate<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return atomicPredicate;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read(); // read the next token, the value

                    switch (propertyName)
                    {
                        case nameof(AtomicPredicate<T>.Value):
                            atomicPredicate.Value = reader.GetString();
                            break;
                        case nameof(AtomicPredicate<T>.Operator):
                            atomicPredicate.Operator = reader.GetString();
                            break;
                        case nameof(AtomicPredicate<T>.MultiSelectValues):
                            atomicPredicate.MultiSelectValues = JsonSerializer.Deserialize<IEnumerable<string>>(ref reader);
                            break;
                        case nameof(AtomicPredicate<T>.Member):
                            atomicPredicate.Member = reader.GetString(); // Set the Member property instead
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        private Expression<Func<T, object>>? CreateExpression(string memberName)
        {
            var parts = memberName.Split('.');
            var param = Expression.Parameter(typeof(T), "x");
            Expression? body = param;

            foreach (var part in parts)
            {
                if (body is null)
                {
                    return null;
                }

                var propertyInfo = body.Type.GetProperty(part);
                if (propertyInfo != null)
                {
                    body = Expression.Property(body, propertyInfo);
                }
                else
                {
                    return null;
                }
            }

            // Wrap the final property access in a Convert, as the Expression must be of type object
            body = Expression.Convert(body, typeof(object));

            return Expression.Lambda<Func<T, object>>(body, param);
        }

        public override void Write(Utf8JsonWriter writer, AtomicPredicate<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(AtomicPredicate<T>.Value), value.Value?.ToString());
            writer.WriteString(nameof(AtomicPredicate<T>.Operator), value.Operator);
            writer.WriteBoolean(nameof(AtomicPredicate<T>.IsMultiSelect), value.IsMultiSelect);
            writer.WritePropertyName(nameof(AtomicPredicate<T>.MultiSelectValues));
            JsonSerializer.Serialize(writer, value.MultiSelectValues, options);
            writer.WriteString(nameof(AtomicPredicate<T>.Member), value.Member); // Serialize Member, not PropertyExpression

            writer.WriteEndObject();
        }
    }
}
