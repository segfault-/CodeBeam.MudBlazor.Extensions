// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
    public interface IFilterDefinition<T>
    {
        Guid Id { get; set; }

        string? Title { get; set; }

        string? Operator { get; set; }

        object? Value { get; set; }

        Type PropertyType { get; set; }

        FieldType FieldType => FieldType.Identify(PropertyType);

        LambdaExpression? PropertyExpression { get; }

        Func<T, bool> GenerateFilterFunction(FilterOptions? filterOptions = null);

        IFilterDefinition<T> Clone();
    }
}
