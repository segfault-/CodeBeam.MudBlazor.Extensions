using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MudExtensions
{
#nullable enable
    public class PredicateUnitJsonTypeInfoResolver<T> : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            Type basePredicateUnitType = typeof(PredicateUnit<T>);
            if (jsonTypeInfo.Type == basePredicateUnitType)
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "$predicate-unit-type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes =
                    {
                        new JsonDerivedType(typeof(AtomicPredicate<T>), "atomic-predicate"),
                        new JsonDerivedType(typeof(CompoundPredicate<T>), "compound-predicate")
                    }
                };
            }

            return jsonTypeInfo;
        }
    }
}
