﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
            if (type is null)
                return false;

            if (type == typeof(string))
                return true;

            return false;
        }

        public static bool IsNumber(Type? type)
        {
            return type is not null && _numericTypes.Contains(type);
        }

        public static bool IsEnum(Type? type)
        {
            if (type is null)
                return false;

            if (type.IsEnum)
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is { IsEnum: true };
        }

        public static bool IsDateTime(Type? type)
        {
            if (type is null)
                return false;

            if (type == typeof(DateTime))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && underlyingType == typeof(DateTime);
        }

        public static bool IsBoolean(Type? type)
        {
            if (type is null)
                return false;

            if (type == typeof(bool))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && underlyingType == typeof(bool);
        }

        public static bool IsGuid(Type? type)
        {
            if (type is null)
                return false;

            if (type == typeof(Guid))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && underlyingType == typeof(Guid);
        }
    }
}
