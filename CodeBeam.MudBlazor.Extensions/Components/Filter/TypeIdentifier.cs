using System.Numerics;

namespace MudExtensions
{
#nullable enable
    internal class TypeIdentifier
    {
        private static readonly HashSet<Type> _numericTypes = new()
        {
            typeof(int),
            typeof(double),
            typeof(decimal),
            typeof(long),
            typeof(short),
            typeof(sbyte),
            typeof(byte),
            typeof(ulong),
            typeof(ushort),
            typeof(uint),
            typeof(float),
            typeof(BigInteger),
            typeof(int?),
            typeof(double?),
            typeof(decimal?),
            typeof(long?),
            typeof(short?),
            typeof(sbyte?),
            typeof(byte?),
            typeof(ulong?),
            typeof(ushort?),
            typeof(uint?),
            typeof(float?),
            typeof(BigInteger?),
        };

        internal static bool IsString(Type? type)
        {
            return IsTypeOrNullableOfType(type, typeof(string));
        }

        public static bool IsNumber(Type? type)
        {
            return type is not null && _numericTypes.Contains(type);
        }

        public static bool IsEnum(Type? type)
        {
            return IsTypeOrNullableOfType(type, t => t.IsEnum);
        }

        public static bool IsDateTime(Type? type)
        {
            return IsTypeOrNullableOfType(type, typeof(DateTime));
        }

        public static bool IsBoolean(Type? type)
        {
            return IsTypeOrNullableOfType(type, typeof(bool));
        }

        public static bool IsGuid(Type? type)
        {
            return IsTypeOrNullableOfType(type, typeof(Guid));
        }

        private static bool IsTypeOrNullableOfType(Type? type, Type targetType)
        {
            if (type == targetType)
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType == targetType;
        }

        private static bool IsTypeOrNullableOfType(Type? type, Func<Type, bool> predicate)
        {
            if (predicate(type))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && predicate(underlyingType);
        }
    }
}
