using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class FilterNode<T> : MudComponentBase
    {
        [CascadingParameter] public MudFilter<T> Filter { get; set; }
        [CascadingParameter] public FilterNode<T> Parent { get; set; }
        [Parameter, EditorRequired] public PredicateUnit<T> PredicateUnit { get; set; }

        protected string ClassName => new CssBuilder("mud-filter-node")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

    }
}
