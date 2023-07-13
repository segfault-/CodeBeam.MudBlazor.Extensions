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
                            var valueString = reader.GetString();

                            // Check if the atomicPredicate.MemberType is not null and is Enum
                            if (atomicPredicate.MemberType is not null && atomicPredicate.MemberType.IsEnum && valueString is not null)
                            {
                                // Parse the string value back to Enum
                                atomicPredicate.Value = Enum.Parse(atomicPredicate.MemberType, valueString);
                            }
                            else
                            {
                                atomicPredicate.Value = valueString;
                            }

                            break;
                        case nameof(AtomicPredicate<T>.Operator):
                            atomicPredicate.Operator = reader.GetString();
                            break;
                        case nameof(AtomicPredicate<T>.MultiSelectValues):
                            atomicPredicate.MultiSelectValues = JsonSerializer.Deserialize<IEnumerable<string>>(ref reader);
                            break;
                        case nameof(AtomicPredicate<T>.Member):
                            atomicPredicate.Member = reader.GetString();
                            break;
                        case nameof(AtomicPredicate<T>.MemberType):
                            string? memberTypeString = reader.GetString();
                            if (memberTypeString != null)
                            {
                                atomicPredicate.MemberType = Type.GetType(memberTypeString);
                            }
                            break;
                    }
                }
            }

            throw new JsonException();
        }


        public override void Write(Utf8JsonWriter writer, AtomicPredicate<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(AtomicPredicate<T>.Value), value.Value?.ToString());
            writer.WriteString(nameof(AtomicPredicate<T>.Operator), value.Operator);
            writer.WriteString(nameof(AtomicPredicate<T>.Member), value.Member);
            writer.WriteString(nameof(AtomicPredicate<T>.MemberType), value.MemberType?.AssemblyQualifiedName);
            writer.WriteEndObject();
        }
    }
}