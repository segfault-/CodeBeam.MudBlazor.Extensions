// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudExtensions
{
#nullable enable
    public class FilterOptions
    {
        public FilterCaseSensitivity FilterCaseSensitivity { get; set; } = FilterCaseSensitivity.Default;

        public static FilterOptions Default { get; } = new();
    }
}
