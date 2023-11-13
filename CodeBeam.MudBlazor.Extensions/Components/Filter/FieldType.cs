// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;

namespace MudExtensions
{
#nullable enable
    public class FieldType
    {
        public Type? InnerType { get; init; }

        public bool IsString { get; init; }

        public bool IsNumber { get; init; }

        public bool IsEnum { get; init; }

        public bool IsDateTime { get; init; }

        public bool IsBoolean { get; init; }

        public bool IsGuid { get; init; }

        public object? Convert(object? value)
        {
            if (value is null)
            {
                return null;
            }

            if (IsString)
            {
                return ConvertToString(value);
            }

            if (IsNumber)
            {
                return ConvertToDouble(value);
            }

            if (IsEnum)
            {
                // Conversion for enums is tricky because you need to know the specific enum type
                // It may be better to handle this case separately
                return value.ToString();
            }

            if (IsDateTime)
            {
                return ConvertToDateTime(value)?.ToUniversalTime();
            }

            if (IsBoolean)
            {
                return ConvertToBoolean(value);
            }

            if (IsGuid)
            {
                return ConvertToGuid(value.ToString());
            }

            return value;
        }

        public static FieldType Identify(Type? type)
        {
            var filedType = new FieldType
            {
                InnerType = type,
                IsString = TypeIdentifier.IsString(type),
                IsNumber = TypeIdentifier.IsNumber(type),
                IsEnum = TypeIdentifier.IsEnum(type),
                IsDateTime = TypeIdentifier.IsDateTime(type),
                IsBoolean = TypeIdentifier.IsBoolean(type),
                IsGuid = TypeIdentifier.IsGuid(type)
            };

            return filedType;
        }

        public static string? ConvertToString(object? value)
        {
            return value as string;
        }

        public static double? ConvertToDouble(object? value)
        {
            if (value is double d)
            {
                return d;
            }
            else if (value is string s)
            {
                if (double.TryParse(s, out var result))
                {
                    return result;
                }
            }
            else if (value is null)
            {
                return null;
            }

            throw new InvalidCastException("Value is not a valid double representation");
        }

        public static bool? ConvertToBoolean(object? value)
        {
            if (value is bool b)
            {
                return b;
            }
            else if (value is string s)
            {
                if (bool.TryParse(s, out var result))
                {
                    return result;
                }
            }
            else if (value is null)
            {
                return null;
            }

            throw new InvalidCastException("Value is not a valid boolean representation");
        }

        public static DateTime? ConvertToDateTime(object? value)
        {
            if (value is DateTime dt)
            {
                return dt;
            }
            else if (value is string s)
            {
                if (DateTime.TryParse(s, out var result))
                {
                    return result;
                }
            }
            else if (value is null)
            {
                return null;
            }

            throw new InvalidCastException("Value is not a valid DateTime representation");
        }

        public static Guid? ConvertToGuid(object? value)
        {
            if (value is Guid g)
            {
                return g;
            }
            else if (value is string str && Guid.TryParse(str, out var guid))
            {
                return guid;
            }
            else if (value is null)
            {
                return null;
            }

            throw new InvalidCastException("Value is not a Guid");
        }

        public IEnumerable GetEnumValues()
        {
            if (InnerType is null)
            {
                throw new InvalidOperationException("The InnerType is not set.");
            }

            Type typeToCheck = Nullable.GetUnderlyingType(InnerType) ?? InnerType;

            if (typeToCheck.IsEnum)
            {
                return Enum.GetValues(typeToCheck);
            }

            throw new InvalidOperationException("The type is not an enum");
        }

    }
}
