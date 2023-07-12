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
                        case nameof(AtomicPredicate<T>.Member):
                            atomicPredicate.Member = reader.GetString();
                            break;
                        case nameof(AtomicPredicate<T>.MemberType):
                            var memberTypeString = reader.GetString();
                            if(memberTypeString is not null)
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